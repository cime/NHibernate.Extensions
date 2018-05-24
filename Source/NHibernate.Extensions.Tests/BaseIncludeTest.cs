﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Engine;
using NHibernate.Extensions.Tests.Entities;
using NHibernate.Tool.hbm2ddl;
using T4FluentNH.Tests;

namespace NHibernate.Extensions.Tests
{
    public class BaseIncludeTest
    {
        protected void ValidateGetEntityResult(EQBPerson petra)
        {
            Assert.AreEqual("Ana", petra.BestFriend.Name);
            Assert.AreEqual("Simon", petra.BestFriend.BestFriend.Name);
            Assert.AreEqual("1", petra.BestFriend.IdentityCard.Code);
            Assert.AreEqual("Rok", petra.BestFriend.BestFriend.BestFriend.Name);
            Assert.AreEqual("Petra", petra.BestFriend.BestFriend.BestFriend.BestFriend.Name);
            Assert.AreEqual("Ana", petra.BestFriend.BestFriend.BestFriend.BestFriend.BestFriend.Name);
            Assert.AreEqual(1, petra.CurrentOwnedVehicles.Count);
            Assert.AreEqual("3", petra.DrivingLicence.Code);
            Assert.AreEqual("4", petra.IdentityCard.Code);
            Assert.AreEqual("System", petra.CreatedBy.UserName);
            Assert.AreEqual(petra.BestFriend.BestFriend, petra.MarriedWith);
            Assert.AreEqual(1, petra.OwnedHouses.Count);
            Assert.AreEqual(2, petra.PreviouslyOwnedVehicles.Count);
            foreach (var wheel in petra.CurrentOwnedVehicles.First().Wheels)
            {
                Assert.AreEqual(235, wheel.Width);
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            var schema = new SchemaExport(NHConfig.Configuration);
            schema.Drop(false, true);
            schema.Create(false, true);
            FillData();
        }

        protected void FillData()
        {
            var system = new EQBUser { UserName = "System" };
            var ana = new EQBPerson { Age = 23, Name = "Ana", CreatedBy = system };
            var rok = new EQBPerson { Age = 24, Name = "Rok", CreatedBy = system };
            var simon = new EQBPerson { Age = 25, Name = "Simon", CreatedBy = system };
            var petra = new EQBPerson { Age = 22, Name = "Petra", CreatedBy = system };

            //Setting best friends
            petra.BestFriend = ana;
            ana.BestFriend = simon;
            simon.BestFriend = rok;
            rok.BestFriend = petra;

            //Setting Identity card
            ana.IdentityCard = new EQBIdentityCard { Code = "1", Owner = ana };
            ana.Identity = new EQBIdentity { Code = "1", Owner = ana };
            rok.IdentityCard = new EQBIdentityCard { Code = "2", Owner = rok };
            rok.Identity = new EQBIdentity { Code = "2", Owner = rok };
            simon.IdentityCard = new EQBIdentityCard { Code = "3", Owner = simon };
            simon.Identity = new EQBIdentity { Code = "3", Owner = simon };
            petra.IdentityCard = new EQBIdentityCard { Code = "4", Owner = petra };
            petra.Identity = new EQBIdentity { Code = "4", Owner = petra };

            //Setting Driving licence
            rok.DrivingLicence = new EQBDrivingLicence { Code = "1", Owner = rok };
            simon.DrivingLicence = new EQBDrivingLicence { Code = "2", Owner = simon };
            petra.DrivingLicence = new EQBDrivingLicence { Code = "3", Owner = petra };

            //Setting MerriedWith
            rok.MarriedWith = ana;
            ana.MarriedWith = rok;

            petra.MarriedWith = simon;
            simon.MarriedWith = petra;

            //Setting Vehicles
            var ferrari = new EQBVehicle { BuildYear = 2002, Model = "Ferrari" };
            ferrari.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 320, Vehicle = ferrari });
            ferrari.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 320, Vehicle = ferrari });
            ferrari.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 260, Vehicle = ferrari });
            ferrari.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 260, Vehicle = ferrari });

            var audi = new EQBVehicle { BuildYear = 2009, Model = "Audi" };
            audi.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 235, Vehicle = audi });
            audi.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 235, Vehicle = audi });
            audi.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 235, Vehicle = audi });
            audi.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 235, Vehicle = audi });

            var bmw = new EQBVehicle { BuildYear = 1993, Model = "Bmw" };
            bmw.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 205, Vehicle = bmw });
            bmw.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 205, Vehicle = bmw });
            bmw.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 205, Vehicle = bmw });
            bmw.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 205, Vehicle = bmw });

            var vw = new EQBVehicle { BuildYear = 2002, Model = "Vw" };
            vw.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 195, Vehicle = vw });
            vw.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 195, Vehicle = vw });
            vw.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 195, Vehicle = vw });
            vw.Wheels.Add(new TestEQBWheel { Diameter = 45, Width = 195, Vehicle = vw });

            petra.PreviouslyOwnedVehicles.Add(vw);
            petra.PreviouslyOwnedVehicles.Add(bmw);
            petra.CurrentOwnedVehicles.Add(audi);
            audi.CurrentOwner = petra;

            simon.PreviouslyOwnedVehicles.Add(bmw);
            simon.PreviouslyOwnedVehicles.Add(audi);
            simon.CurrentOwnedVehicles.Add(ferrari);
            ferrari.CurrentOwner = simon;

            //Setting Houses
            var house1 = new EQBHouse { Address = "Address1" };
            var house2 = new EQBHouse { Address = "Address2" };

            house1.Owners.Add(ana);
            ana.OwnedHouses.Add(house1);
            house1.Owners.Add(rok);
            rok.OwnedHouses.Add(house1);

            house2.Owners.Add(simon);
            simon.OwnedHouses.Add(house2);
            house2.Owners.Add(petra);
            petra.OwnedHouses.Add(house2);

            //Saving entities
            using (var session = NHConfig.OpenSession())
            {
                session.Save(petra);
                session.Save(rok);
                session.Save(simon);
                session.Save(ana);
                session.Flush();
            }
        }
    }
}
