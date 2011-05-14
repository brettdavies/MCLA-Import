using System;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Collections.Generic;
using KSULax.Entities;
using KSULax.Dal;
using KSULax.Logic.Import;
using HtmlAgilityPack;

namespace MCLAImport
{
    public partial class Service1
    {
        public static void Main(string[] args)
        {
            importMCLA();

            //importMCLATop23();
            //importCLTop25();
        }

        private static void importMCLATop23()
        {
            string url = "http://mclamag.com/top23/"+DateTime.Now.Year;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(httpResponse.GetResponseStream());

            short pollSourceId = 1;

            DateTime pollDate = DateTime.Parse(doc.DocumentNode.SelectSingleNode("//h3").InnerText);

            string pollUrl = url +@"/" + doc.DocumentNode.SelectSingleNode("//li/a[contains(@class, 'active')]").Attributes["href"].Value;

            short rank = 0;
            short index = 1;
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//ol[@class='div2']/li/span/a/img[@src]"))
            {
                HtmlAttribute att = link.Attributes["src"];
                if (att.Value.Contains("kennesaw_state"))
                {
                    rank = index;
                    break;
                }

                index++;
            }

            if (rank > 0)
            {
                DataBL dbl = new DataBL();
                dbl.UpdatePoll(new TeamRankingBE
                    {
                        Date = pollDate,
                        pollSource = pollSourceId,
                        Rank = rank,
                        Url = pollUrl
                    }
                );
            }
        }

        private static void importCLTop25()
        {
            string root = "http://www.collegelax.us";
            string url = root + "/polls.php";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(httpResponse.GetResponseStream());

            short pollSourceId = 2;

            string h4 = doc.DocumentNode.SelectSingleNode("//h4").InnerText;
            DateTime pollDate = DateTime.Parse(h4.Substring(h4.LastIndexOf(' '))+ '/' + DateTime.Now.Year );

            string pollUrl = root + doc.DocumentNode.SelectSingleNode("//ul[@class='News']/li/a[@href]").Attributes["href"].Value;

            short rank = 0;

            HtmlNode tr = doc.DocumentNode.SelectSingleNode("//tr/td/a[contains(@href,'kennesaw_state')]").ParentNode.ParentNode;

            if (short.TryParse(tr.FirstChild.InnerText, out rank))
            {
                DataBL dbl = new DataBL();
                dbl.UpdatePoll(new TeamRankingBE
                {
                    Date = pollDate,
                    pollSource = pollSourceId,
                    Rank = rank,
                    Url = pollUrl
                }
                );
            }
        }

        #region MCLA
        private static void importMCLA()
        {
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=game&team=kennesaw_state";
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=roster&team=kennesaw_state";
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=game&start=02/12/2011&end=02/14/2011&team=kennesaw_state";
            //string url = "http://mcla.us/scores/games/6353/";
            //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse();

            MCLA mcla = new MCLA();

            XmlDocument responseXML = new XmlDocument();

            //responseXML.Load(httpResponse.GetResponseStream());
            responseXML.Load(@"C:\Users\Brett\Documents\Visual Studio 2010\Projects\MCLAImport\xml\Roster.xml");
            var players = roster.Deserialize(responseXML.InnerXml);
            var pbeLst = GetPlayerBE(players.player);

            if (null != pbeLst)
            {
                mcla.UpdatePlayers(pbeLst);
            }

            //responseXML.Load(httpResponse.GetResponseStream());
            responseXML.Load(@"C:\Users\Brett\Documents\Visual Studio 2010\Projects\MCLAImport\xml\Schedule.xml");
            var sch = schedule.Deserialize(responseXML.InnerXml);
            var geLst = GetGameBE(sch.game);
            var pgbeLst = GetGameStatBE(sch.game);

            if (null != geLst)
            {
                mcla.UpdateGames(geLst);

                if (null != pgbeLst)
                {
                    mcla.UpdateGameStatByGame(pgbeLst);
                }
            }
        }

        private static List<GameStatBE> GetGameStatBE(List<scheduleGame> games)
        {
            var pgbeLst = new List<GameStatBE>();

            foreach (scheduleGame game in games)
            {
                bool _isHome = game.home_team_slug.Equals("kennesaw_state");

                pgbeLst.AddRange(GetGameStatBE(_isHome ? game.home_players : game.away_players, game.id, game.game_season_id));
            }

            return pgbeLst;
        }

        private static List<GameStatBE> GetGameStatBE(List<PlayerGameStat> players, ushort gameID, ushort seasonID)
        {
            var pgbeLst = new List<GameStatBE>();

            foreach (PlayerGameStat player in players)
            {
                pgbeLst.Add(new GameStatBE
                {
                    Assists = player.assists,
                    GameID = gameID,
                    Goals = player.goals,
                    GoalsAgainst = player.ga,
                    PlayerID = player.id,
                    Saves = player.saves,
                    SeasonID = seasonID
                });
            }

            return pgbeLst;
        }

        private static List<GameBE> GetGameBE(List<scheduleGame> games)
        {
            var gbeLst = new List<GameBE>();

            foreach (scheduleGame game in games)
            {
                gbeLst.Add(GetGameBE(game));
            }

            return gbeLst;
        }

        private static GameBE GetGameBE(scheduleGame game)
        {
            var ge = new GameBE
            {
                AwayTeamScore = game.away_team_score,
                AwayTeamSlug = game.away_team_slug,
                Datetime = DateTime.Parse(game.game_date + ' ' + game.game_season_id + ' '+ game.game_time),
                SeasonID = (short)game.game_season_id,
                Status = game.game_status,
                Type = game.game_type,
                HomeTeamScore = game.home_team_score,
                HomeTeamSlug = game.home_team_slug,
                ID = (short)game.id,
                Venue = game.venue
            };

            return ge;
        }

        private static List<PlayerBE> GetPlayerBE(List<rosterPlayer> players)
        {
            var pbeLst = new List<PlayerBE>();

            foreach (rosterPlayer player in players)
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
            pbe.Team = player.season.team;

            return pbe;
        }

        #endregion MCLA
    }
}