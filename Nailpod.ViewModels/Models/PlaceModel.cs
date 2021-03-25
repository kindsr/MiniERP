#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using Nailpod.Data;
using Nailpod.Services;

namespace Nailpod.Models
{
    public class PlaceModel : ObservableObject
    {
        static public PlaceModel CreateEmpty() => new PlaceModel { PlaceID = -1, IsEmpty = true };

        public long PlaceID { get; set; }
        public long CustomerID { get; set; }

        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }

        public DateTimeOffset? InstallDate { get; set; }
        public string DelYn { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }

        public bool IsNew => PlaceID <= 0;
        public string CountryName => LookupTablesProxy.Instance.GetCountry(CountryCode);

        public CustomerModel Customer { get; set; }

        public string FullAddress
        {
            get
            {
                return $"{Address} {Address2}\n{City}, {Region} {PostalCode}\n{CountryName}";
            }
        }

        public override void Merge(ObservableObject source)
        {
            if (source is PlaceModel model)
            {
                Merge(model);
            }
        }

        public void Merge(PlaceModel source)
        {
            if (source != null)
            {
                PlaceID = source.PlaceID;
                CustomerID = source.CustomerID;
                Address = source.Address;
                Address2 = source.Address2;
                City = source.City;
                Region = source.Region;
                CountryCode = source.CountryCode;
                PostalCode = source.PostalCode;
                Phone = source.Phone;
                InstallDate = source.InstallDate;
                DelYn = source.DelYn;
                CreatedOn = source.CreatedOn;
                LastModifiedOn = source.LastModifiedOn;
                Customer = source.Customer;
            }
        }

        public override string ToString()
        {
            return PlaceID.ToString();
        }
    }
}
