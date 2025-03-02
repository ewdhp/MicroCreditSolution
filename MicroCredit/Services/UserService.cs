using System;
using System.Collections.Generic;
using System.Linq;
using MicroCredit.Data;
using MicroCredit.Models;

namespace MicroCredit.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetCurrentUser(Guid currentUserId)
        {
            return _context.Users.Find(currentUserId);
        }

        public User GetUserById(Guid id)
        {
            return _context.Users.Find(id);
        }

        public User GetUserByFingerprint(string fingerprint)
        {
            return _context.Users.FirstOrDefault(u => u.Fingerprint == fingerprint);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
