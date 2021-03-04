using AutoMapper;
using Business.Repository.IRepository;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class HotelRoomRepository : IHotelRoomRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public HotelRoomRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<HotelRoomDTO> CreateHotelRoom(HotelRoomDTO hotelRoomDTO)
        {
            HotelRoom hotelRoom = _mapper.Map<HotelRoomDTO, HotelRoom>(hotelRoomDTO);
            hotelRoom.CreatedDate = DateTime.UtcNow;
            hotelRoom.CreatedBy = "";

            var addedHotelRoom = await _db.HotelRooms.AddAsync(hotelRoom);
            await _db.SaveChangesAsync();

            return _mapper.Map<HotelRoom, HotelRoomDTO>(addedHotelRoom.Entity);
        }

        public async Task<int> DeleteHotelRoom(int roomId)
        {
            var roomDetails = await _db.HotelRooms.FindAsync(roomId);
            if(roomDetails != null)
            {
                var allImages = await _db.HotelRoomImages.Where(x => x.RoomId == roomId).ToListAsync();



                //foreach (var image in allImages)
                //{
                //    if (File.Exists(image.RoomImageUrl))
                //    {
                //        File.Delete(image.RoomImageUrl);
                //    }
                //}

                _db.HotelRoomImages.RemoveRange(allImages);
                _db.HotelRooms.Remove(roomDetails);
                return await _db.SaveChangesAsync();
            }

            return 0;
        }

        public Task<int> DeletePetarsHotelRoom(string roomId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HotelRoomDTO>> GetAllHotelRooms(string checkInDateStr, string checkOutDatestr)
        {
            try
            {
                IEnumerable<HotelRoomDTO> hotelRoomDTOs =
                            _mapper.Map<IEnumerable<HotelRoom>, IEnumerable<HotelRoomDTO>>
                            (_db.HotelRooms.Include(x => x.HotelRoomImages));
                if (!string.IsNullOrEmpty(checkInDateStr) && !string.IsNullOrEmpty(checkOutDatestr))
                {
                    foreach (HotelRoomDTO hotelRoom in hotelRoomDTOs)
                    {
                        hotelRoom.IsBooked = await IsRoomBooked(hotelRoom.Id, checkInDateStr, checkOutDatestr);
                    }
                }
                return hotelRoomDTOs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<HotelRoomDTO> GetHotelRoom(int roomId, string checkInDateStr, string checkOutDatestr)
        {
            try
            {
                HotelRoomDTO hotelRoom = _mapper.Map<HotelRoom, HotelRoomDTO>(
                    await _db.HotelRooms.Include(x => x.HotelRoomImages).FirstOrDefaultAsync(x => x.Id == roomId));

                if (!string.IsNullOrEmpty(checkInDateStr) && !string.IsNullOrEmpty(checkOutDatestr))
                {
                    hotelRoom.IsBooked = await IsRoomBooked(roomId, checkInDateStr, checkOutDatestr);
                }

                return hotelRoom;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<bool> IsRoomBooked(int roomId, string checkInDateStr, string checkOutDateStr)
        {

            try
            {
                if(!string.IsNullOrEmpty(checkOutDateStr) && !string.IsNullOrEmpty(checkInDateStr))
                {
                    DateTime checkInDate = DateTime.Parse(checkInDateStr);
                    DateTime checkOutDate = DateTime.Parse(checkOutDateStr);

                    var existingBooking = await _db.RoomOrderDetails.Where(x => x.RoomId == roomId && x.IsPaymentSuccessfull &&
                        // Check if checkin date that user wants does not fall in between any dataes for that room that is booked
                        ((checkInDate < x.CheckOutDate && checkInDate >= x.CheckInDate)
                        // Check if checkin date that user wants does not fall in between any dataes for that room that is booked
                        || (checkOutDate.Date > x.CheckInDate && checkInDate.Date <= x.CheckInDate.Date)
                        )).FirstOrDefaultAsync();

                    if(existingBooking != null)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        // If unique returns null else return room obj
        public async Task<HotelRoomDTO> IsRoomUnique(string name, int roomId = 0)
        {
            try
            {
                if(roomId == 0)
                {
                    HotelRoomDTO hotelRoom = _mapper.Map<HotelRoom, HotelRoomDTO>(
                    await _db.HotelRooms.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower()));

                    return hotelRoom;
                }
                else
                {
                    HotelRoomDTO hotelRoom = _mapper.Map<HotelRoom, HotelRoomDTO>(
                    await _db.HotelRooms.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower()
                    && x.Id != roomId));

                    return hotelRoom;
                }


            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<HotelRoomDTO> UpdateHotelRoom(int roomId, HotelRoomDTO hotelRoomDTO)
        {
            try
            {
                if (roomId == hotelRoomDTO.Id)
                {
                    // Valid
                    HotelRoom roomDetails = await _db.HotelRooms.FindAsync(roomId);
                    HotelRoom room = _mapper.Map<HotelRoomDTO, HotelRoom>(hotelRoomDTO, roomDetails);

                    room.UpdatedBy = "";
                    room.UpdatedDate = DateTime.UtcNow;

                    var updatedRoom = _db.HotelRooms.Update(room);
                    await _db.SaveChangesAsync();

                    return _mapper.Map<HotelRoom, HotelRoomDTO>(updatedRoom.Entity);
                }
                else
                {
                    //invalid
                    return null;
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }
    }
}
