using AutoMapper;
using SIGNAL_R_CHAT.Infrastructure;
using SIGNAL_R_CHAT.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SIGNAL_R_CHAT.API.ViewModels;

namespace SIGNAL_R_CHAT.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public readonly static List<PersonViewModel> _Connections = new();
        public readonly static List<WorkGroupViewModel> _WorkGroups = new();
        private readonly static Dictionary<string, string> _ConnectionsMap = new();

        private readonly Context _context;
        private readonly IMapper _mapper;
        
        public ChatHub(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task SendPrivate(string receiverName, string text)
        {
            if (_ConnectionsMap.TryGetValue(receiverName, out string personId))
            {
                // Who is the sender;
                var sender = _Connections.Where(u => u.PersonName == IdentityName).First();

                if (!string.IsNullOrEmpty(text.Trim()))
                {
                    // Build the message
                    var messageViewModel = new MessageViewModel
                    {
                        Text = Regex.Replace(text, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                        From = sender.PersonName,
                        To = "",
                        DateTime = DateTime.Now.ToLongTimeString()
                    };

                    // Send the message
                    await Clients.Client(personId).SendAsync("newMessage", messageViewModel);
                    await Clients.Caller.SendAsync("newMessage", messageViewModel);
                }
            }
        }

        public async Task SendToRoom(string workGroupName, string text)
        {
            try
            {
                var person = _context.Persons.Where(u => u.Name == IdentityName).FirstOrDefault();
                var workGroup = _context.WorkGroups.Where(r => r.Name == workGroupName).FirstOrDefault();

                if (!string.IsNullOrEmpty(text.Trim()))
                {
                    // Create and save message in database
                    var message = new Message()
                    {
                        Text = Regex.Replace(text, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                        FromPerson = person,
                        ToGroup = workGroup,
                        DateTime = DateTime.Now
                    };
                    _context.Messages.Add(message);
                    _context.SaveChanges();

                    // Broadcast the message
                    var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                    await Clients.Group(workGroupName).SendAsync("newMessage", messageViewModel);
                }
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("onError", "Message not send! Message should be 1-500 characters.");
            }
        }

        public async Task Join(string workGroupName)
        {
            try
            {
                var person = _Connections.Where(u => u.PersonName == IdentityName).FirstOrDefault();
                if (person != null && person.CurrentWorkGroup != workGroupName)
                {
                    // Remove user from others list
                    if (!string.IsNullOrEmpty(person.CurrentWorkGroup))
                        await Clients.OthersInGroup(person.CurrentWorkGroup).SendAsync("removePerson", person);

                    // Join to new chat room
                    await Leave(person.CurrentWorkGroup);
                    await Groups.AddToGroupAsync(Context.ConnectionId, workGroupName);
                    person.CurrentWorkGroup = workGroupName;

                    // Tell others to update their list of users
                    await Clients.OthersInGroup(workGroupName).SendAsync("addPerson", person);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "You failed to join the chat room!" + ex.Message);
            }
        }

        public async Task Leave(string workGroupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, workGroupName);
        }

        public async Task CreateRoom(string workGroupName)
        {
            try
            {

                // Accept: Letters, numbers and one space between words.
                Match match = Regex.Match(workGroupName, @"^\w+( \w+)*$");
                if (!match.Success)
                {
                    await Clients.Caller.SendAsync("onError", "Invalid room name!\nRoom name must contain only letters and numbers.");
                }
                else if (workGroupName.Length < 5 || workGroupName.Length > 100)
                {
                    await Clients.Caller.SendAsync("onError", "Room name must be between 5-100 characters!");
                }
                else if (_context.WorkGroups.Any(r => r.Name == workGroupName))
                {
                    await Clients.Caller.SendAsync("onError", "Another chat room with this name exists");
                }
                else
                {
                    // Create and save chat room in database
                    var user = _context.Persons.Where(u => u.Name == IdentityName).FirstOrDefault();
                    var workGroup = new WorkGroup()
                    {
                        Name = workGroupName,
                        Admin = user
                    };
                    _context.WorkGroups.Add(workGroup);
                    _context.SaveChanges();

                    if (workGroup != null)
                    {
                        // Update room list
                        var workGroupViewModel = _mapper.Map<WorkGroup, WorkGroupViewModel>(workGroup);
                        _WorkGroups.Add(workGroupViewModel);
                        await Clients.All.SendAsync("addWorkGroup", workGroupViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Couldn't create work group: " + ex.Message);
            }
        }

        public async Task DeleteWorkGroup(string workGroupName)
        {
            try
            {
                // Delete from database
                var workGroup = _context.WorkGroups.Include(r => r.Admin)
                    .Where(r => r.Name == workGroupName && r.Admin.Name == IdentityName).FirstOrDefault();
                _context.WorkGroups.Remove(workGroup);
                _context.SaveChanges();

                // Delete from list
                var workGroupViewModel = _WorkGroups.First(r => r.WorkGroupName == workGroupName);
                _WorkGroups.Remove(workGroupViewModel);

                // Move users back to Lobby
                await Clients.Group(workGroupName).SendAsync("onRoomDeleted", string.Format("Room {0} has been deleted.\nYou are now moved to the Lobby!", workGroupName));

                // Tell all users to update their room list
                await Clients.All.SendAsync("removeWorkGroup", workGroupViewModel);
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("onError", "Can't delete this chat room. Only owner can delete this room.");
            }
        }

        public IEnumerable<WorkGroupViewModel> GetRooms()
        {
            // First run?
            if (_WorkGroups.Count == 0)
            {
                foreach (var workGroup in _context.WorkGroups)
                {
                    var workGroupViewModel = _mapper.Map<WorkGroup, WorkGroupViewModel>(workGroup);
                    _WorkGroups.Add(workGroupViewModel);
                }
            }

            return _WorkGroups.ToList();
        }

        public IEnumerable<PersonViewModel> GetPersonsInWorkGroup(string workGroupName)
        {
            return _Connections.Where(u => u.CurrentWorkGroup == workGroupName).ToList();
        }

        public IEnumerable<MessageViewModel> GetMessageHistory(string workGroupName)
        {
            var messageHistory = _context.Messages.Where(m => m.ToGroup.Name == workGroupName)
                    .Include(m => m.FromPerson)
                    .Include(m => m.ToGroup)
                    .OrderByDescending(m => m.DateTime)
                    .Take(20)
                    .AsEnumerable()
                    .Reverse()
                    .ToList();

            return _mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(messageHistory);
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                var person = _context.Persons.Where(u => u.Name == IdentityName).FirstOrDefault();
                var personViewModel = _mapper.Map<Person, PersonViewModel>(person);
                personViewModel.CurrentWorkGroup = "";

                if (!_Connections.Any(u => u.PersonName == IdentityName))
                {
                    _Connections.Add(personViewModel);
                    _ConnectionsMap.Add(IdentityName, Context.ConnectionId);
                }

                Clients.Caller.SendAsync("getProfileInfo", person.Name);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var person = _Connections.Where(u => u.PersonName == IdentityName).First();
                _Connections.Remove(person);

                // Tell other users to remove you from their list
                Clients.OthersInGroup(person.CurrentWorkGroup).SendAsync("removePerson", person);

                // Remove mapping
                _ConnectionsMap.Remove(person.PersonName);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }

            return base.OnDisconnectedAsync(exception);
        }

        private string IdentityName
        {
            get { return Context.User.Identity.Name; }
        }

    }
}
