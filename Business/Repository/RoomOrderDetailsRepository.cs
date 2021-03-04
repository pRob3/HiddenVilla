﻿using AutoMapper;
using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class RoomOrderDetailsRepository : IRoomOrderDetailsRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoomOrderDetailsRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<RoomOrderDetailsDTO> Create(RoomOrderDetailsDTO details)
        {
            try
            {
                details.CheckInDate = details.CheckInDate.Date;
                details.CheckOutDate = details.CheckOutDate.Date;

                var roomOrder = _mapper.Map<RoomOrderDetailsDTO, RoomOrderDetails>(details);
                roomOrder.Status = SD.Status_Pending;

                var result = await _context.RoomOrderDetails.AddAsync(roomOrder);
                await _context.SaveChangesAsync();

                return _mapper.Map<RoomOrderDetails, RoomOrderDetailsDTO>(result.Entity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<RoomOrderDetailsDTO>> GetAllRoomOrderDetails()
        {
            try
            {
                IEnumerable<RoomOrderDetailsDTO> roomOrders = _mapper.Map<IEnumerable<RoomOrderDetails>, IEnumerable<RoomOrderDetailsDTO>>
                    (_context.RoomOrderDetails.Include(u => u.HotelRoom));

                return roomOrders;
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public async Task<RoomOrderDetailsDTO> GetRoomOrderDetail(int roomOrderId)
        {
            try
            {
                RoomOrderDetails roomOrder = await _context.RoomOrderDetails
                    .Include(u => u.HotelRoom)
                    .ThenInclude(x => x.HotelRoomImages)
                    .FirstOrDefaultAsync(u => u.Id==roomOrderId);

                RoomOrderDetailsDTO roomOrderDetailsDTO = _mapper.Map<RoomOrderDetails, RoomOrderDetailsDTO>(roomOrder);

                roomOrderDetailsDTO.HotelRoomDTO.TotalDays = roomOrderDetailsDTO.CheckOutDate
                    .Subtract(roomOrderDetailsDTO.CheckInDate).Days;

                return roomOrderDetailsDTO;
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public async Task<bool> IsRoomBooked(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            var status = false;

            var existingBooking = await _context.RoomOrderDetails.Where(x => x.RoomId == roomId && x.IsPaymentSuccessfull &&
            // Check if checkin date that user wants does not fall in between any dataes for that room that is booked
            (checkInDate < x.CheckOutDate && checkInDate > x.CheckInDate
            // Check if checkin date that user wants does not fall in between any dataes for that room that is booked
            || checkOutDate.Date > x.CheckInDate && checkInDate.Date < x.CheckInDate.Date
            )).FirstOrDefaultAsync();

            if(existingBooking != null)
            {
                status = true;
            }

            return status;
        }

        public Task<RoomOrderDetailsDTO> MarkPaymentSuccessful(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderStatus(int roomOrderId, string status)
        {
            throw new NotImplementedException();
        }
    }
}