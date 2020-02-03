using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface ICKImageService { 
        Task Upload(CKImage ckimage);
        Task Update(CKImage ckimageUpdate);
        Task Delete(int Id);
        Task GetCKImage(int Id);
        // IEnumerable<CKImage> GetAllImages();
        Task<List<CKImage>> GetAllImages();

    }   
}
