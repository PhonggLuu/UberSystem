﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UberSystem.Domain.Entities;
using UberSystem.Common.Enums;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;
namespace UberSystem.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Add(User user)
        {
            try
            {
                var userRepository = _unitOfWork.Repository<User>();
                var customerRepository = _unitOfWork.Repository<Customer>();
                var driverRepository = _unitOfWork.Repository<Driver>();
                if (user is not null)
                {
                    await _unitOfWork.BeginTransaction();
                    // check duplicate user
                    var existedUser = await userRepository.GetAsync(u => u.Id == user.Id || u.Email == user.Email);
                    if (existedUser is not null) throw new Exception("User already exists.");

                    await userRepository.InsertAsync(user);

                    // add customer or driver into tables
                    if (user.Role == UserRole.Customer)
                    {
                        var customer = _mapper.Map<Customer>(user);
                        await customerRepository.InsertAsync(customer);
                    }
                    else if (user.Role == UserRole.Driver)
                    {
                        var driver = _mapper.Map<Driver>(user);
                        await driverRepository.InsertAsync(driver);
                    }
                    await _unitOfWork.CommitTransaction();
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public Task CheckPasswordAsync(User user)
        {
            throw new NotImplementedException();
		}

        public async Task<User> FindById(long id)
        {
            return await _unitOfWork.Repository<User>().FindAsync(id);
        }

        public async Task<User?> FindByEmail(string email)
        {
            return await _unitOfWork.Repository<User>().GetAsync(u => u.Email == email);
		}

		public async Task<User?> FindByUsername(string username)
		{
			return await _unitOfWork.Repository<User>().GetAsync(u => u.UserName == username);
		}


		public async Task<IEnumerable<User>> GetCustomers()
        {
            var userRepository = _unitOfWork.Repository<User>();
            var users = await userRepository.GetAllAsync();

            var customers = users.Where(u => u.Role == (int)UserRole.Customer);
            return customers;
        }

        public async Task<bool> Login(User user)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                var UserRepos = _unitOfWork.Repository<User>();
                var objUser = await UserRepos.FindAsync(user.Email);
                if (objUser == null)
                    return false;
                if (objUser.Password != user.Password)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> Login(string email, string password)
        {
            var userRepository = _unitOfWork.Repository<User>();
            var users = await userRepository.GetAllAsync();

            var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);
            return user;
        }

        public async Task<User?> GetByVerificationToken(string token)
        {
            var userRepository = _unitOfWork.Repository<User>();
            return await userRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        }

        public async Task Update(User user)
        {
            try
            {
                var userRepository = _unitOfWork.Repository<User>();
                if (user is not null)
                {
                    await _unitOfWork.BeginTransaction();
                    await userRepository.UpdateAsync(user);
                    await _unitOfWork.CommitTransaction();
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

		public async Task Delete(User user)
		{
			try
			{
				var userRepository = _unitOfWork.Repository<User>();
				if (user is not null)
				{
					await _unitOfWork.BeginTransaction();
					await userRepository.DeleteAsync(user);
					await _unitOfWork.CommitTransaction();
				}
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransaction();
				throw;
			}
		}
	}
}

