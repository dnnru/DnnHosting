using System.Collections.Generic;
using Italliance.Modules.DnnHosting.Models;

namespace Italliance.Modules.DnnHosting.ViewModels
{
    public class AdminEmailViewModel
    {
        public AdminEmailViewModel()
        {
            ExpiredClients = new List<ClientDto>();
            PreExpiredClients = new List<ClientDto>();
        }

        public List<ClientDto> ExpiredClients { get; set; }
        public List<ClientDto> PreExpiredClients { get; set; }

        public bool HasExpiredClients => ExpiredClients.Count > 0;
        public bool HasPreExpiredClients => PreExpiredClients.Count > 0;
    }
}