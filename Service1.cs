using System;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Collections.Generic;
using KSULax.Entities;
using KSULax.Models;
using KSULax.Dal;
using KSULax.Logic;

namespace MCLAImport
{
    public partial class Service1
    {
        private static PlayerBL _playerBL;

        public static void Main(string[] args)
        {
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=game&team=kennesaw_state";
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=roster&team=kennesaw_state";
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=game&start=02/12/2011&end=02/14/2011&team=kennesaw_state";
            //string url = "http://mcla.us/scores/games/6353/";
            //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse();

            XmlDocument responseXML = new XmlDocument();
            
            //responseXML.Load(httpResponse.GetResponseStream());
            responseXML.Load(@"C:\Users\Brett\Documents\Visual Studio 2010\Projects\MCLAImport\xml\Roster.xml");
            //StringBuilder sb = new StringBuilder(responseXML.InnerXml);
            roster players = roster.Deserialize(responseXML.InnerXml);

            List<PlayerBE> pbeLst = GetPlayerBE(players);

            List<PlayerEntity> peLst = GetPlayerEntity(pbeLst);

            using (var ent = new KSULaxEntities())
            {
                _playerBL = new PlayerBL(ent);
                PlayerBE pbe;

                foreach (PlayerEntity pe in peLst)
                {
                    var pse = pe.PlayerSeason.GetEnumerator().Current;
                    
                    pbe = _playerBL.PlayerByID(pe.id, pse.season_id);
                    
                    if (null == pbe)
                    {
                        ent.AddToPlayerSet(pe);
                        ent.AddToPlayerSeasonSet(pse);
                    }
                    else
                    {
                        pbe.FirstName = pe.first;
                        pe.highschool;
                        pe.homestate;
                        pe.hometown;
                        pe.last;
                        pe.major;
                        pe.middle;
                    }
                }

                ent.SaveChanges();
            }

            //responseXML.Load(@"C:\Users\Brett\Documents\Visual Studio 2010\Projects\MCLAImport\xml\Schedule.xml");
            //schedule sch = schedule.Deserialize(responseXML.InnerXml);

        }

        private static List<PlayerEntity> GetPlayerEntity(List<PlayerBE> pbeLst)
        {
            var peLst = new List<PlayerEntity>();

            foreach (PlayerBE pbe in pbeLst)
            {
                peLst.Add(GetPlayerEntity(pbe));
            }

            return peLst;
        }

        private static PlayerEntity GetPlayerEntity(PlayerBE pbe)
        {
            var pe = new PlayerEntity();

            pe.first = pbe.FirstName;
            pe.highschool = pbe.HighSchool;
            pe.homestate = pbe.HomeState;
            pe.hometown = pbe.Hometown;
            pe.id = (short)pbe.PlayerID;
            pe.last = pbe.LastName;
            pe.major = pbe.Major;
            pe.middle = pbe.MiddleName;

            var pse = new PlayerSeasonEntity();

            pse.@class = pbe.ClassYr;
            pse.eligibility = pbe.EligibilityYr;
            pse.height = (short)pbe.Height;
            pse.jersey = (short)pbe.JerseyNum;
            pse.player_id = (short)pbe.PlayerID;
            pse.position = pbe.Position;
            pse.season_id = (short)pbe.SeasonID;
            pse.weight = (short)pbe.Weight;

            pe.PlayerSeason.Add(pse);

            return pe;
        }

        private static List<PlayerBE> GetPlayerBE(roster players)
        {
            var pbeLst = new List<PlayerBE>();

            foreach (rosterPlayer player in players.player)
            {
                pbeLst.Add(GetPlayerBE(player));
            }

            return pbeLst;
        }

        private static PlayerBE GetPlayerBE(rosterPlayer player)
        {
            PlayerBE pbe = new PlayerBE();

            pbe.FirstName = player.first;
            pbe.HighSchool = player.highschool;
            pbe.HomeState = player.homestate;
            pbe.Hometown = player.hometown;
            pbe.PlayerID = player.id;
            pbe.LastName = player.last;
            pbe.Major = player.major;
            pbe.MiddleName = player.middle;

            pbe.ClassYr = player.season.@class;
            pbe.EligibilityYr = player.season.eligibility;
            pbe.Height = player.season.height;
            pbe.SeasonID = player.season.id;
            pbe.JerseyNum = player.season.jersey;
            pbe.Position = player.season.position;
            pbe.Weight = player.season.weight;

            //player.season.team;

            return pbe;
        }
    }
}