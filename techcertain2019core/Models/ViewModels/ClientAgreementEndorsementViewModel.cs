﻿using System;


namespace techcertain2019core.Models.ViewModels
{
        public class ClientAgreementEndorsementViewModel : BaseViewModel
        {
            public Guid ClientAgreementEndorsementID { get; set; }

            public string Name { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }

            public string OrderNumber { get; set; }

            public Guid ClientAgreementID { get; set; }

        }
    }