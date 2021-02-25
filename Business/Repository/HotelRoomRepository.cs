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
        private readonly IMapper mapper;

        public HotelRoomRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            this.mapper = mapper;
        }

        public async Task<HotelRoomDTO> CreateHotelRoom(HotelRoomDTO hotelRoomDTO)
        {
            HotelRoom hotelRoom = mapper.Map<HotelRoomDTO, HotelRoom>(hotelRoomDTO);
            hotelRoom.CreatedDate = DateTime.UtcNow;
            hotelRoom.CreatedBy = "";

            var addedHotelRoom = await _db.HotelRooms.AddAsync(hotelRoom);
            await _db.SaveChangesAsync();

            return mapper.Map<HotelRoom, HotelRoomDTO>(addedHotelRoom.Entity);
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

        public async Task<IEnumerable<HotelRoomDTO>> GetAllHotelRooms(string checkInDateStr, string checkOutDateStr)
        {
            try
            {
                IEnumerable<HotelRoomDTO> hotelRoomDTOs = 
                    mapper.Map<IEnumerable<HotelRoom>, IEnumerable<HotelRoomDTO>> 
                    (_db.HotelRooms.Include(x => x.HotelRoomImages));

                return hotelRoomDTOs;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task<HotelRoomDTO> GetHotelRoom(int roomId, string checkInDateStr, string checkOutDateStr)
        {
            try
            {
                HotelRoomDTO hotelRoom = mapper.Map<HotelRoom, HotelRoomDTO>(
                    await _db.HotelRooms.Include(x => x.HotelRoomImages).FirstOrDefaultAsync(x => x.Id == roomId));

                return hotelRoom;

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        // If unique returns null else return room obj
        public async Task<HotelRoomDTO> IsRoomUnique(string name, int roomId = 0)
        {
            try
            {
                if(roomId == 0)
                {
                    HotelRoomDTO hotelRoom = mapper.Map<HotelRoom, HotelRoomDTO>(
                    await _db.HotelRooms.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower()));

                    return hotelRoom;
                }
                else
                {
                    HotelRoomDTO hotelRoom = mapper.Map<HotelRoom, HotelRoomDTO>(
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
                    HotelRoom room = mapper.Map<HotelRoomDTO, HotelRoom>(hotelRoomDTO, roomDetails);

                    room.UpdatedBy = "";
                    room.UpdatedDate = DateTime.UtcNow;

                    var updatedRoom = _db.HotelRooms.Update(room);
                    await _db.SaveChangesAsync();

                    return mapper.Map<HotelRoom, HotelRoomDTO>(updatedRoom.Entity);
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
