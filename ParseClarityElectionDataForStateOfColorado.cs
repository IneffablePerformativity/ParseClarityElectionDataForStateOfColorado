/*
 * ParseClarityElectionDataForStateOfColorado.cs
 *
 * which code and results I will archive at:
 * https://github.com/IneffablePerformativity
 * 
 * "To the extent possible under law, Ineffable Performativity has waived all copyright and related or neighboring rights to
 * The C# program Georgia2020ElectionFraud.cs and resultant outputs.
 * This work is published from: United States."
 * 
 * This work is offered as per license: http://creativecommons.org/publicdomain/zero/1.0/
 *
 * 
 * This application has mined out and presents solid proof of 2020 POTUS ELECTION FRAUD here:
 * LocationBeingStudied = "StateOfColorado"
 * 
 * 
 * This work can easily be tweaked for other locations that use CLARITY election system.
 * String constants matching Contest names must change;
 * How to distinguish Party may change.
 * GrainTag may change.
 * XML hierarchy may change
 * 
 * 
 * Goal: Parsing the "Clarity" type of 2020 Election data for State of Colorado.
 * via https://results.enr.clarityelections.com/CO/105975/web.264614/#/summary
 * Where I manually downloaded detailxml.zip, extracted detail.xml,
 * and renamed it: StateOfColorado-detail.xml
 * 
 * to demonstrate any inverse republicanism::trump relationship
 * as was described for Milwaukee County Wisconsin in an article at:
 * https://www.thegatewaypundit.com/2020/11/caught-part-3-impossible-ballot-ratio-found-milwaukee-results-change-wisconsin-election-30000-votes-switched-president-trump-biden/
 * 
 * 
 * This application comes on the heels of my similar successful app,
 * which was also saved there on github.com/IneffablePerformativity:
 * ParseMilwaukeeCountyWiVotes.cs
 * That app showed a very clear 3% skew of POTUS race from lower races.
 * 
 * 
 * I am cloning an app that did the CLARITY data for state of Georgia.
 * But that data did NOT show POTUS race to be skewed from lower races,
 * so I did not proceed to the plotting step therein. Maybe this time.
 * 
 * 
 * The least grained item in State of Georgia was called a County.
 * The least grained item in Oakland County MI is called a Precinct.
 * One cannot compare at higher grained level Contests to one another
 * because they do not consist of all the very same individual voters.
 * 
 * 
 * Georgia did NOT give me TotalBallots cast per county.
 * So I approximated it as the sum of all POTUS choices.
 * Oakland County MI gave me TotalBallots in many ways.
 * 
 * Georgia had 4 vote types ~(MailIn/ElectionDay/Advance/Provisional).
 * Oakland County MI has only 2 vote types (under Choice):
 * Choice.VoteType = "Election"
 * Choice.VoteType = "Absentee"
 * Counting VoteTypes separately created big code bloat I did not use.
 * 
 * New PLAN: I will simplify and more generally name data structures:
 * Contest[] -> Choice[] -> Grain[] -> SumVotes (across all VoteType)
 * 
 * Georgia had "(Dem)" or "(Rep)" in Choice text (name of candidate).
 * Here, Choice has OPTIONAL attribute "party": "DEM", "REP", others.
 * 
 * Here is a new concept: Contest may be "Straight Party Ticket"
 * Which I see alongside normal Contest names like:
 * "Electors of President and Vice-President of the United States"
 * "United States Senator"
 * "Representative in Congress 8th District"
 * "Representative in State Legislature 26th District"
 * and many lower contest names.
 * 
 * Planning: Such votes can credit POTUS and US SENATORS,
 * but applying to local races would be more complicated.
 * Also "Representative in Congress..." is Federal level,
 * 
 * 
 * Here is a new concept: Contest.VoteType  = "Undervotes"
 * Here is a new concept: Contest.VoteType  = "Overvotes"
 * Which pair are seen in all contests, before multiple "Choice" nodes;
 * Note the reuse of same xml name VoteType at this higher DOM level.
 * 
 * Wikipedia:
 * - An overvote occurs when one votes for more than the maximum number of selections allowed in a contest.
 * The result is a spoiled vote which is not included in the final tally.
 * - An undervote occurs when the number of choices selected by a voter in a contest is less than the maximum number allowed for that contest or when no selection is made for a single choice contest.
 * - Undervotes combined with overvotes (known as residual votes) can be an academic indicator in evaluating the accuracy of a voting system when recording voter intent.
 * 
 * 
 * 
 * Prior App for GEORGIA comments:
 * 
 * Oh, but I had used the totalBallotsCast, also totalRegistered by Ward;
 * Whereas these Georgia results do not share that information by county.
 * I will estimate totalBallotsCast in county = POTUS (Rep)+(Dem)+(Lib);
 * I.e., as if nobody had voted without marking any presidential choice.
 * 
 * The smallest granuarity unit is the COUNTY (as was WARD in prior app).
 * Counties are the plot independent variable, ordered by "republicanism".
 * I wonder, should I include POTUS race in the measure of republicanism?
 * That might smooth better than only averaging the contested lower races.
 * 
 * Now, specific to Colorado:
 * Code was a total-shoe-in from the Oakland County WI version of APP.
 * 
 * I resorted CSV file by Trump Bonus, examined a couple spikes,
 * like ADAMS, and they seem to have very low sum of votes cast.
 * 
 * Therfore I added a new output type to log, see final routine.
 * It disclosed no UNCONTESTED race, no reason to tweak divisor.
 * 
 * The Bonus Statistics:
 * Across the 64 localities of StateOfColorado, the BIDEN vote differs from other DEMOCRAT races as: mean = PLUS 3.01% of total votes in locality, with standard deviation = 0.26%.
 * Across the 64 localities of StateOfColorado, the TRUMP vote differs from other REPUBLICAN races as: mean = MINUS 1.15% of total votes in locality, with standard deviation = 0.37%.
 * 
 * BIDEN BONUS = PLUS 3.01%, StdDev = 0.26%; TRUMP BONUS = MINUS 1.15%, StdDev = 0.37%
 * Can those stats be so uniform without proving systematic vote manipulation?
 * On to the task...
 */


using System;
using System.Collections.Generic;
using System.Drawing; // Must add a reference to System.Drawing.dll
using System.Drawing.Imaging; // for ImageFormat
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;


namespace ParseClarityElectionDataForStateOfColorado
{
	
	class Choice // AKA Candidate
	{
		public Choice(string party)
		{
			this.party = party;
		}
		public string party = "";
		public int votes = 0;
	}
	
	class Contest // AKA Race
	{
		public Dictionary<string,Choice> choices = new Dictionary<string,Choice>();
	}
	
	class Grain // AKA Precinct, County, Ward, etc.
	{
		public Grain(string locality)
		{
			this.locality = locality;
		}
		public string locality = "";
		public Dictionary<string,Contest> contests = new Dictionary<string,Contest>();
	}
	
	class ShrodingersCat // needed to make a reference to N sums, which one unknown yet
	{
		public int rep;
		public int dem;
		public int etc;
	}

	class Program
	{
		static string LocationBeingStudied = "StateOfColorado";

		static string LocationBeingStudiedClearly = "State Of Colorado";

		static string gitHubRepositoryURL = "https://github.com/IneffablePerformativity/ParseClarityElectionDataForStateOfColorado";

		static string gitHubRepositoryShortened = "https://bit.ly/3kZjYIH";
		
		static string finalConclusion = "YES, FRAUD: BIDEN BONUS = PLUS 3.01%, StdDev 0.26%; TRUMP BONUS = MINUS 1.15%, StdDev 0.37%";

		static bool discardStraightPartyVotes = true; // affects bonus, csv, more than plot

		
		// Grains are smallest locality, perhaps a Ward, county, precinct...

		const string GrainTag = "County"; // XML tag name e.g., Precinct, County, Ward, etc.

		
		
		// inputting phase

		
		static string projectDirectoryPath = @"C:\A\SharpDevelop\ParseClarityElectionDataFor" + LocationBeingStudied;
		
		static string inputXmlFileName = LocationBeingStudied + "-detail.xml"; // sitting there

		// This is the top-level store of all grains, held by locality name
		
		static Dictionary<string,Grain> grains = new Dictionary<string,Grain>();

		
		// Analysis Phase

		
		// Some versions can foreknow ballotsCast, e.g., in input data or a website.
		// Some versions must compute ballotsCast, e.g., as sum of all POTUS votes.
		// Just for a self-check; Not needed in computations:
		
		const int ballotsCast = 999999; // Empirically, from this current input file

		
		// outputting phase

		
		static string DateTimeStr = DateTime.Now.ToString("yyyyMMddTHHmmss");
		
		// Log outputs exploratory, debug data:

		static string logFilePath = @"C:\A\" + DateTimeStr + "_ParseClarityElectionDataFor" + LocationBeingStudied + "_log.txt";
		static TextWriter twLog = null;

		// a favorite output idiom

		static void say(string msg)
		{
			if(twLog != null)
				twLog.WriteLine(msg);
			else
				Console.WriteLine(msg);
		}

		// Csv outputs voting data to visualize in Excel

		static string csvFilePath = @"C:\A\" + DateTimeStr + "_ParseClarityElectionDataFor" + LocationBeingStudied + "_csv.csv";
		static TextWriter twCsv = null;

		static void csv(string msg)
		{
			if(twCsv != null)
				twCsv.WriteLine(msg);
		}

		// this cleans csv output text

		static Regex regexNonAlnum = new Regex(@"\W", RegexOptions.Compiled);
		
		static char [] caComma = { ',' };

		
		// Png outputs my own bitmap for fine grained control of data display:

		static string pngFilePath = @"C:\A\" + DateTimeStr + "_ParseClarityElectionDataFor" + LocationBeingStudied + "_png.png";

		
		// Affects pen width choices!

		const int imageSizeX = 1920 * 2;
		const int imageSizeY = 1080 * 2;
		const int nBorder = 80 * 2; // keep mod 10 == 0
		
		const int plotSizeX = imageSizeX - 2 * nBorder;
		const int plotSizeY = imageSizeY - 2 * nBorder + 1; // so plotSizeY mod 10 == 1 for 11 graticules
		
		
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
			// TODO: Implement Functionality Here
			
			using(twLog = File.CreateText(logFilePath))
				using(twCsv = File.CreateText(csvFilePath))
			{
				try { doit(); } catch(Exception e) {say(e.ToString());}
			}

			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}

		
		static void doit()
		{
			// Phase one -- Inputting data
			
			say("discardStraightPartyVotes = " + discardStraightPartyVotes);
			
			inputDetailXmlFile();

			// diagnosticDump(); // optional, not needed
			

			// Phase two -- Analyze data
			
			LetsRaceTowardTheDesideratum();

			
			// Phase three -- Output data
			
			// I scale [0.0% to 1.0%] into ppm = Parts Per Million [0 to 1000000].
			// all ppm are computed with divisor = totalBallots in grain locality.
			
			// "Bonus to Biden" will plot as mountains above the central graticule.
			// "Taken from Trump" will plot as valleys below the central graticule.
			
			// How does one express all that clearly in column header text?
			if(twCsv != null)
				csv("ordering,ppmRepPotus,ppmDemPotus,1M-ppmDemPotus,ppmRepOther,ppmDemOther,1M-ppmDemOther,BonusToTrump,BonusToBiden,totalBallots,locality");

			OutputTheCsvResults();
			
			OutputTheStatistics();

			OutputThePlotResults();

			OutputGrainContestChoiceParty(); // new since Colorado version
		}

		
		static void inputDetailXmlFile()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.Combine(projectDirectoryPath,inputXmlFileName));

			XmlNode root = doc.DocumentElement;
			
			// XML descent
			
			XmlNodeList xnlContests = root.SelectNodes("Contest");
			foreach(XmlNode xnContest in xnlContests)
			{
				string contest = xnContest.Attributes["text"].Value;
				
				// Having just run through all to see the lay of it,
				// Only process contest of interest to my statistic.
				
				switch(contest)
				{
						// N/A in Colorado:
						//case "Straight Party Ticket":
						//	if(discardStraightPartyVotes)
						//		continue;
						//	break; // Else, Process this category

						// sum of POTUS votes == total ballots in Oakland County:
					case "Presidential Electors":
						// sum of US Senator votes == total ballots in Oakland County:
						break; // Process this category

					case "United States Senator":
						break; // Process this category
						// sum of these 4 REP races == total ballots in Oakland County:

					case "Representative to the 117th United States Congress - District 1":
					case "Representative to the 117th United States Congress - District 2":
					case "Representative to the 117th United States Congress - District 3":
					case "Representative to the 117th United States Congress - District 4":
					case "Representative to the 117th United States Congress - District 5":
					case "Representative to the 117th United States Congress - District 6":
					case "Representative to the 117th United States Congress - District 7":
						break; // Process this category

					default:
						continue; // discard all others
				}

				XmlNodeList xnlChoices = xnContest.SelectNodes("Choice");
				foreach(XmlNode xnChoice in xnlChoices)
				{
					string choice = xnChoice.Attributes["text"].Value;
					string party = "";
					try { party = xnChoice.Attributes["party"].Value; } catch(Exception) { };


					// 1. Real votes
					XmlNodeList xnlGrains = xnChoice.SelectNodes("VoteType/" + GrainTag);
					foreach(XmlNode xnGrain in xnlGrains)
					{
						string grain = xnGrain.Attributes["name"].Value;
						string votes = xnGrain.Attributes["votes"].Value;
						
						ponderInput(contest, choice, party, grain, votes);
					}
				}

				// Having seen it, now not needed
				//
				//{
				//	// 2. Residual votes
				//	// <VoteType name="Undervotes"...
				//	XmlNodeList xnlGrains = xnContest.SelectNodes("VoteType[@name='Overvotes' or @name='Undervotes']/" + GrainTag);
				//	foreach(XmlNode xnGrain in xnlGrains)
				//	{
				//		string grain = xnGrain.Attributes["name"].Value;
				//		string votes = xnGrain.Attributes["votes"].Value;
				//
				//		ponderInput(contest, "Residual", "", grain, votes);
				//	}
				//}
			}
			// Wow. That was way easier.
		}
		
		static void ponderInput(string contest, string choice, string party, string grain, string votes)
		{
			// say(contest + "::" + choice + "::" + party + "::" + grain + "::" + votes);
			
			int nVotes = int.Parse(votes);
			
			// XML ascent
			
			Grain thisGrain = null;
			if(grains.ContainsKey(grain))
				thisGrain = grains[grain];
			else
				grains.Add(grain, thisGrain = new Grain(grain));
			
			Contest thisContest = null;
			if(thisGrain.contests.ContainsKey(contest))
				thisContest = thisGrain.contests[contest];
			else
				thisGrain.contests.Add(contest, thisContest = new Contest());
			
			Choice thisChoice = null;
			if(thisContest.choices.ContainsKey(choice))
				thisChoice = thisContest.choices[choice];
			else
				thisContest.choices.Add(choice, thisChoice = new Choice(party));
			
			thisChoice.votes += nVotes;
			
			// Wow. Another easy peasy.
		}

		
		/*
		 * I think having made the effort to parse and count Residual counts,
		 * all contests->...->grains->votes should sum to the identical ballotsCast.
		 * 
		 * <VoterTurnout totalVoters="1035172" ballotsCast="775379" voterTurnout="74.90">
		 * 
		 * No, I think not now:
		 * Some local races have fewer of the statewide ballots,
		 * but I did get some lines to sum up within a Grain:
		 * 
		 * input, under TotalVotes:
		 * <Precinct name="Addison Township, Precinct 1" totalVoters="1930" ballotsCast="1590" ...
		 * 
		 * versus output:
		 * Addison Township, Precinct 1::Straight Party Ticket::1590
		 * Addison Township, Precinct 1::Electors of President and Vice-President of the United States::1590
		 * Addison Township, Precinct 1::United States Senator::1590
		 * 
		 * however, I see some near-multiples and multiples too:
		 * Addison Township, Precinct 1::Member of the State Board of Education::3179
		 * Addison Township, Precinct 1::Regent of the University of Michigan::3180
		 * 
		 * So I think my diagnostic failed to sum-up to a high enough level...
		 * 
		 * Fixed it.
		 * 
		 * Did not snag all of Federals; Many others are don't cares:
		 * GOOD TOTAL: Straight Party Ticket
		 * GOOD TOTAL: Electors of President and Vice-President of the United States
		 * GOOD TOTAL: United States Senator
		 * hand work:
		 * Line 1: Representative in Congress 8th District = 164088
		 * Line 26: Representative in Congress 9th District = 137999
		 * Line 49: Representative in Congress 11th District = 293394
		 * Line 156: Representative in Congress 14th District = 179898
		 * 164088 + 137999 + 293394  + 179898 = 775379, matches ballotsCast = 775379
		 */

		
		static void diagnosticDump()
		{
			// because of the inverted order of grain<-->contest, sum up:
			Dictionary<string,int> contestSums = new Dictionary<string, int>();

			foreach(KeyValuePair<string,Grain>kvp1 in grains)
			{
				string g = kvp1.Key;
				Grain grain = kvp1.Value;
				foreach(KeyValuePair<string,Contest>kvp2 in grain.contests)
				{
					string c = kvp2.Key;
					Contest contest = kvp2.Value;
					int sumVotes = 0;
					foreach(KeyValuePair<string,Choice>kvp3 in contest.choices)
					{
						string n = kvp3.Key;
						Choice choice = kvp3.Value;
						sumVotes += choice.votes;
					}
					if(contestSums.ContainsKey(c))
						contestSums[c] += sumVotes;
					else
						contestSums.Add(c, sumVotes);
				}
			}
			
			int nGood = 0;
			int nBad = 0;
			foreach(KeyValuePair<string,int>kvp in contestSums)
			{
				if(kvp.Value == ballotsCast)
				{
					// say("GOOD TOTAL: " + kvp.Key); // see which 15?
					nGood++;
				}
				else
				{
					// say(kvp.Key + " = " + kvp.Value); // How bad?
					nBad++;
				}
			}
			// say("nGood = " + nGood);
			// say("nBad = " + nBad);
			//nGood = 15
			//nBad = 260
			// I'll take that as a big win
		}
		

		// This will collect output lines to sort for csv.
		// After sort, re-parse column data to draw graph.
		
		static List<string> csvLines = new List<string>();

		
		// My God! Thank You, My God!
		// The Excel plot reveals a VERY STRAIGHT LINE
		// revealing the algorithm favoring Joe Biden.
		// So that I must see and report mean, std dev.
		//
		// That 1.5% was in the StateOfColorado data.
		// for generality, output both statistics.
		//
		// (But MilwaukeeCountyWI had a sloping line.)
		// But output these as a signed percentage:

		// These hold the counts for mean, std dev:
		
		static List<int> BidenBonuses = new List<int>();
		static List<int> TrumpBonuses = new List<int>();
		
		
		// Isaiah 41:15 “Behold, I will make thee a new sharp threshing instrument having teeth: thou shalt thresh the mountains, and beat them small, and shalt make the hills as chaff.”
		
		
		static void LetsRaceTowardTheDesideratum()
		{
			StringBuilder sb = new StringBuilder();
			
			// Grains are plotted along the Abscissa, the X axis.
			// Each outer loop can prepare one csv/plot data out.
			// That is because I already learned the ballotsCast.
			// No rather, compute a total ballots for each grain.

			// I will learn contest long before I learn party.

			ShrodingersCat [] four = new ShrodingersCat[4];

			four[0] = new ShrodingersCat(); // Straight
			four[1] = new ShrodingersCat(); // President
			four[2] = new ShrodingersCat(); // Senate
			four[3] = new ShrodingersCat(); // Congress

			int which = 0;
			
			foreach(KeyValuePair<string,Grain>kvp1 in grains)
			{
				string g = kvp1.Key; // county, ward, precinct name
				Grain grain = kvp1.Value;

				// Middle loop splits up 2 contest types: (POTUS vs. LOWER)
				// Well, actually, into the four cats I created just above.

				foreach(KeyValuePair<string,Contest>kvp2 in grain.contests)
				{
					string c = kvp2.Key;
					Contest contest = kvp2.Value;

					switch(c)
					{
							// N/A in Colorado
							//case "Straight Party Ticket":
							//	// I should add to all possible races.
							//	// These will only dilute the symptom.
							//	which = 0;
							//	break;

						case "Presidential Electors":
							which = 1;
							break;

						case "United States Senator":
							which = 2;
							break;


						case "Representative to the 117th United States Congress - District 1":
						case "Representative to the 117th United States Congress - District 2":
						case "Representative to the 117th United States Congress - District 3":
						case "Representative to the 117th United States Congress - District 4":
						case "Representative to the 117th United States Congress - District 5":
						case "Representative to the 117th United States Congress - District 6":
						case "Representative to the 117th United States Congress - District 7":
							which = 3;
							break;
					}

					// Inner loop splits up 2 choice types: (REP vs. DEM).
					
					// In Oakland County MI data, these came as XML attributes:

					foreach(KeyValuePair<string,Choice>kvp3 in contest.choices)
					{
						string n = kvp3.Key;
						Choice choice = kvp3.Value;
						
						switch(choice.party)
						{
							case "DEM":
								four[which].dem += choice.votes;
								break;
							case "REP":
								four[which].rep += choice.votes;
								break;
							default:
								four[which].etc += choice.votes;
								break;
						}
					}
				}

				// build an output line
				
				
				// fyi: csv("ordering,ppmRepPotus,ppmDemPotus,1M-ppmDemPotus,ppmRepOther,ppmDemOther,1M-ppmDemOther,BonusToTrump,BonusToBiden,totalBallots,locality");

				// sum up VOTES:
				int RepPotus = four[0].rep + four[1].rep;
				int DemPotus = four[0].dem + four[1].dem;
				int EtcPotus = four[0].etc + four[1].etc;
				int totalBallots = RepPotus + DemPotus + EtcPotus;

				int RepOther = four[0].rep + (four[2].rep + four[3].rep) / 2;
				int DemOther = four[0].dem + (four[2].dem + four[3].dem) / 2;
				
				
				int ppmRepPotus = (int)(1000000L * RepPotus / totalBallots);
				int ppmDemPotus = (int)(1000000L * DemPotus / totalBallots);

				int ppmRepOther = (int)(1000000L * RepOther / totalBallots);
				int ppmDemOther = (int)(1000000L * DemOther / totalBallots);
				
				int ppmTrumpBonus = ppmRepPotus - ppmRepOther;
				int ppmBidenBonus = ppmDemPotus - ppmDemOther;
				
				// these are signed, around zero, but scaled in PPM

				BidenBonuses.Add(ppmBidenBonus); // for later mean, std dev
				TrumpBonuses.Add(ppmTrumpBonus); // for later mean, std dev

				// Revise the shift==steal metrics for clarity:
				// "Taken From Trump" sounds nice but confuses.
				
				// on PPM, plot full scale of 1M = 100%
				// desire bonus plot full scale = +/-5%
				// so scale up by 10.
				
				int bonusToTrump = 500000 + 10 * ppmTrumpBonus; // both plot up==positive
				int bonusToBiden = 500000 + 10 * ppmBidenBonus; // both plot up==positive
				
				// This ordering method follows (only Other, of only Rep) no Dem, no Potus,
				// as Original Milwaukee article says shifts proportional to Republicanism.
				
				int ppmOrdering = ppmRepOther;

				sb.Clear();
				// field[0]
				sb.Append(ppmOrdering.ToString().PadLeft(7)); sb.Append(',');

				// field[1]
				sb.Append(ppmRepPotus.ToString().PadLeft(7)); sb.Append(',');
				// field[2]
				sb.Append(ppmDemPotus.ToString().PadLeft(7)); sb.Append(',');
				sb.Append((1000000 - ppmDemPotus).ToString().PadLeft(7)); sb.Append(',');

				// field[4]
				sb.Append(ppmRepOther.ToString().PadLeft(7)); sb.Append(',');
				// field[5]
				sb.Append(ppmDemOther.ToString().PadLeft(7)); sb.Append(',');
				sb.Append((1000000 - ppmDemOther).ToString().PadLeft(7)); sb.Append(',');

				// field[7]
				sb.Append(bonusToTrump.ToString().PadLeft(7)); sb.Append(',');
				// field[8]
				sb.Append(bonusToBiden.ToString().PadLeft(7)); sb.Append(',');

				sb.Append(totalBallots.ToString().PadLeft(7)); sb.Append(',');

				sb.Append(regexNonAlnum.Replace(grain.locality, ""));

				csvLines.Add(sb.ToString());
			}
		}
		
		static void OutputTheCsvResults()
		{
			csvLines.Sort();
			
			foreach(string line in csvLines)
				csv(line);
		}
		
		static void OutputTheStatistics()
		{

			// Wow. I call this evidence tight: 1.5% of totalBallots was added to Joe across the entire county!
			// The Democrat Vote PPM Shift (Potus Dem minus Other Dems) in all 506 localities of Oakland County Michigan is: mean=15140, stdDev=2531.
			
			// Do once for Biden
			{
				int sum = 0;
				foreach(int i in BidenBonuses) // signed around zero, scaled*1M
					sum += i;
				double mean = (double)sum / BidenBonuses.Count;
				
				double sumSquares = 0.0;
				foreach(int i in BidenBonuses)
					sumSquares += (mean-i)*(mean-i);
				double stdDev = Math.Sqrt(sumSquares / BidenBonuses.Count);
				
				// divide by 100 first to keep 2 dec plcs in pct
				int imean = (int)Math.Round(mean/100);
				string sign = (imean<0?"MINUS ":"PLUS ");
				imean = Math.Abs(imean);
				int istdDev = (int)Math.Round(stdDev/100);
				double dmean = imean / 100.0; // now as Percentage
				double dstdDev = istdDev / 100.0; // now as Percentage

				say("Across the " + BidenBonuses.Count + " localities of " + LocationBeingStudied
				    + ", the BIDEN vote differs from other DEMOCRAT races as: mean = "
				    + sign + dmean + "% of total votes in locality, with standard deviation = " + dstdDev + "%.");
			}
			
			// Do once for Trump
			{
				int sum = 0;
				foreach(int i in TrumpBonuses) // signed around zero, scaled*1M
					sum += i;
				double mean = (double)sum / TrumpBonuses.Count;
				
				double sumSquares = 0.0;
				foreach(int i in TrumpBonuses)
					sumSquares += (mean-i)*(mean-i);
				double stdDev = Math.Sqrt(sumSquares / TrumpBonuses.Count);
				
				// divide by 100 first to keep 2 dec plcs in pct
				int imean = (int)Math.Round(mean/100);
				string sign = (imean<0?"MINUS ":"PLUS ");
				imean = Math.Abs(imean);
				int istdDev = (int)Math.Round(stdDev/100);
				double dmean = imean / 100.0; // now as Percentage
				double dstdDev = istdDev / 100.0; // now as Percentage
				
				say("Across the " + TrumpBonuses.Count + " localities of " + LocationBeingStudied
				    + ", the TRUMP vote differs from other REPUBLICAN races as: mean = "
				    + sign + dmean + "% of total votes in locality, with standard deviation = " + dstdDev + "%.");
			}
		}
		
		
		// Habakkuk 2:2 "And the Lord answered me, and said, Write the vision, and make it plain upon tables, that he may run that readeth it."

		
		static void OutputThePlotResults()
		{
			Bitmap bmp = new Bitmap(imageSizeX, imageSizeY);
			bmp.SetResolution(92, 92);
			Graphics gBmp = Graphics.FromImage(bmp);
			
			Brush whiteBrush = new SolidBrush(Color.White);
			gBmp.FillRectangle(whiteBrush, 0, 0, imageSizeX, imageSizeY); // x,y,w,h

			// again:csv("ordering,ppmRepPotus,ppmDemPotus,1M-ppmDemPotus,ppmRepOther,ppmDemOther,1M-ppmDemOther,BonusToTrump,BonusToBiden,totalBallots,locality");
			
			// goggled the official party colors.
			// Rep Red
			//RGB: 233 20 29
			//HSL: 357° 84% 50%
			//Hex: #E9141D
			// Dem Blue
			//RGB: 0 21 188
			//HSL: 233° 100% 37%
			//Hex: #0015BC

			const int votesLineWidth = 12; // at 1920 * 2
			Pen redTrumpLinePen = new Pen(Color.FromArgb(233, 20, 29), votesLineWidth);
			Pen blueBidenLinePen = new Pen(Color.FromArgb(0, 21, 188), votesLineWidth);

			const int bonusLineWidth = 4; // at 1920 * 2
			Pen GreenBidenBonusLinePen = new Pen(Color.Green, bonusLineWidth);
			Pen orangeTrumpBonusLinePen = new Pen(Color.Orange, bonusLineWidth);

			Pen blackGraticulePen = new Pen(Color.Black, 8); // at 1920* 2
			Pen blackGatCenterLinePen = new Pen(Color.Black, 16); // at 1920* 2

			// Wiki: Pastels in HSV have high value and low to intermediate saturation.
			// Made my color conversions at https://serennu.com/colour/hsltorgb.php
			//Trying Pastel HSL: 357 40 90
			//HSL: 357° 40% 90%
			//RGB: 240 219 220
			//Hex: #F0DBDC
			//Trying Pastel HSL: 233 40 90
			//HSL: 233° 40% 90%
			//RGB: 219 222 240
			//Hex: #DBDEF0
			
			Brush redRepublicansBrush = new SolidBrush(Color.FromArgb(240, 219, 220));
			Brush blueDemocratsBrush = new SolidBrush(Color.FromArgb(219, 222, 240));

			// plot the areas

			{
				// gBmp.FillRectangle(); // brush,x,y,w,h

				// on first app, I had worried over proportional widths and pixel boundaries.
				// today, will I even let extreme edge data go under the vertical graticules.
				float barWidth = (float)plotSizeX / csvLines.Count;
				float leftOfBar = nBorder;

				// factor in the border offsets ahead of loop
				int zeroForDescendingY = nBorder;
				//int zeroForBonusesY = (1 + 5*plotSizeY/10) + nBorder;
				int zeroForAscendingY = plotSizeY + nBorder;

				foreach(string line in csvLines)
				{
					string[] fields = line.Split(caComma);
					int ppmRepOther = int.Parse(fields[4]);
					int ppmDemOther = int.Parse(fields[5]);
					float RepHeight = (float)plotSizeY * ppmRepOther / 1000000;
					float DemHeight = (float)plotSizeY * ppmDemOther / 1000000;
					gBmp.FillRectangle(blueDemocratsBrush, leftOfBar, zeroForDescendingY, barWidth, DemHeight);
					gBmp.FillRectangle(redRepublicansBrush, leftOfBar, zeroForAscendingY - RepHeight, barWidth, RepHeight);
					
					leftOfBar += barWidth;
				}
			}

			// draw graticule

			{

				// Draw the eleven 10% horizontal graticule lines:
				for(int y = 1; y <= plotSizeY; y += plotSizeY/10)
				{
					if(y == 1 + 5*plotSizeY/10)
						gBmp.DrawLine(blackGatCenterLinePen, nBorder, nBorder + y, nBorder + plotSizeX, nBorder + y); // x1, y1, x2, y2
					else
						gBmp.DrawLine(blackGraticulePen, nBorder, nBorder + y, nBorder + plotSizeX, nBorder + y); // x1, y1, x2, y2
				}

				// Draw two vertical boundary lines
				for(int x = 0; x <= plotSizeX; x += plotSizeX)
				{
					// looked like high water trouser legs. Fudging + 1:
					gBmp.DrawLine(blackGraticulePen, nBorder + x, nBorder, nBorder + x, nBorder + plotSizeY + 1); // x1, y1, x2, y2
				}
			}
			
			// plot the lines
			
			{
				// on first app, I had worried over proportional widths and pixel boundaries.
				// today, will I even let extreme edge data go under the vertical graticules.
				float barWidth = (float)plotSizeX / csvLines.Count;
				float leftOfBar = nBorder;

				// factor in the border offsets ahead of loop
				int zeroForDescendingY = nBorder;
				int zeroForBonusesY = (1 + 5*plotSizeY/10) + nBorder;
				int zeroForAscendingY = plotSizeY + nBorder;

				float priorMidBarX = 0;
				float priorTrumpY = 0;
				float priorBidenY = 0;
				float priorTrumpBonusY = 0;
				float priorBidenBonusY = 0;
				
				foreach(string line in csvLines)
				{
					string[] fields = line.Split(caComma);
					int ppmRepPotus = int.Parse(fields[1]);
					int ppmDemPotus = int.Parse(fields[2]);
					int bonusToTrump = int.Parse(fields[7]) - 500000; // was scaled up *10 and shifted +500K
					int bonusToBiden = int.Parse(fields[8]) - 500000; // was scaled up *10 and shifted +500K
					float RepHeight = (float)plotSizeY * ppmRepPotus / 1000000;
					float DemHeight = (float)plotSizeY * ppmDemPotus / 1000000;
					float trumpY = zeroForAscendingY - RepHeight;
					float bidenY = zeroForDescendingY + DemHeight;
					// minus here, as +bonus plots upward, bu Windows GDI +y is downward:
					float trumpBonusY = zeroForBonusesY - (float)plotSizeY * bonusToTrump / 1000000;
					float bidenBonusY = zeroForBonusesY - (float)plotSizeY * bonusToBiden / 1000000;
					float midBarX = leftOfBar + barWidth / 2;

					if(priorMidBarX != 0)
					{
						// draw bonuses first
						gBmp.DrawLine(GreenBidenBonusLinePen, priorMidBarX, priorBidenBonusY, midBarX, bidenBonusY); // pen, x1, y1, x2, y2
						gBmp.DrawLine(orangeTrumpBonusLinePen, priorMidBarX, priorTrumpBonusY, midBarX, trumpBonusY); // pen, x1, y1, x2, y2

						// draw votes second on top
						gBmp.DrawLine(blueBidenLinePen, priorMidBarX, priorBidenY, midBarX, bidenY); // pen, x1, y1, x2, y2
						gBmp.DrawLine(redTrumpLinePen, priorMidBarX, priorTrumpY, midBarX, trumpY); // pen, x1, y1, x2, y2

					}
					
					priorMidBarX = midBarX;
					priorTrumpY = trumpY;
					priorBidenY = bidenY;
					priorTrumpBonusY = trumpBonusY;
					priorBidenBonusY = bidenBonusY;
					
					leftOfBar += barWidth;
				}
			}
			
			// draw the legends

			{
				// Finally, Make some captions right over the bar graphs
				
				// don't write right on top of ( i * plotSizeY / 10) = graticules.

				Brush blackTextBrush = new SolidBrush(Color.Black);
				
				Font bigFont = new Font("Arial Black", 80);
				int bigFontHeight = (int)bigFont.GetHeight(gBmp);

				Font smallFont = new Font("Arial Black", 40);
				int smallFontHeight = (int)smallFont.GetHeight(gBmp);

				Font unambiguousFont = new Font("Consolas", 35);
				int unambiguousFontHeight = (int)smallFont.GetHeight(gBmp);

				int yHeadine = nBorder + 5 * plotSizeY / 100 - bigFontHeight / 2;
				string Headine = "\"CLARITY\" election data for " + LocationBeingStudiedClearly;
				gBmp.DrawString(Headine, bigFont, blackTextBrush, (imageSizeX-gBmp.MeasureString(Headine, bigFont).Width) / 2, yHeadine);

				int yMeaningLabel = nBorder + 15 * plotSizeY / 100 - smallFontHeight / 2;
				string MeaningLabel = "Republicans are RED, Democrats are BLUE; Lines show % POTUS votes; Areas show % average NON-POTUS votes.";
				gBmp.DrawString(MeaningLabel, smallFont, blackTextBrush, (imageSizeX-gBmp.MeasureString(MeaningLabel, smallFont).Width) / 2, yMeaningLabel);

				int yFraudLabel = nBorder + 25 * plotSizeY / 100 - smallFontHeight / 2;
				string FraudLabel = "ELECTION FRAUD CLUE IS WHENEVER RED AND/OR BLUE LINES SYSTEMATICALLY DO NOT TRACK THEIR AREA EDGE.";
				gBmp.DrawString(FraudLabel, smallFont, blackTextBrush, (imageSizeX-gBmp.MeasureString(FraudLabel, smallFont).Width) / 2, yFraudLabel);

				int YQEDLabel = nBorder + 65 * plotSizeY / 100 - smallFontHeight / 2;
				string QEDLabel = "Conclusion: " + finalConclusion;
				gBmp.DrawString(QEDLabel, smallFont, blackTextBrush, (imageSizeX-gBmp.MeasureString(QEDLabel, smallFont).Width) / 2, YQEDLabel);
				
				int YBidenLabel = nBorder + 75 * plotSizeY / 100 - smallFontHeight / 2;
				string BidenLabel = "The GREEN line amplifies any % Bonus To BIDEN, above (or loss below) fat center graticule. Full scale = +/-5%";
				gBmp.DrawString(BidenLabel, smallFont, blackTextBrush, (imageSizeX-gBmp.MeasureString(BidenLabel, smallFont).Width) / 2, YBidenLabel);
				
				int YTrumpLabel = nBorder + 85 * plotSizeY / 100 - smallFontHeight / 2;
				string TrumpLabel = "The ORANGE line amplifies any % Bonus To TRUMP, above (or loss below) fat center graticule. Full scale = +/-5%";
				gBmp.DrawString(TrumpLabel, smallFont, blackTextBrush, (imageSizeX-gBmp.MeasureString(TrumpLabel, smallFont).Width) / 2, YTrumpLabel);

				int yAboveLabel = nBorder + 95 * plotSizeY / 100 - smallFontHeight / 2;
				string AboveLabel = "Localities (\"" + GrainTag + "\") are plotted <-- left to right --> by ascending Republicanism.";
				gBmp.DrawString(AboveLabel, smallFont, blackTextBrush, (imageSizeX-gBmp.MeasureString(AboveLabel, smallFont).Width) / 2, yAboveLabel);

				int yGitHubLabel = nBorder + 103 * plotSizeY / 100 - smallFontHeight / 2;
				string GitHubLabel = "Open Source: " + gitHubRepositoryShortened + " = " + gitHubRepositoryURL;
				gBmp.DrawString(GitHubLabel, unambiguousFont, blackTextBrush, (imageSizeX-gBmp.MeasureString(GitHubLabel, unambiguousFont).Width) / 2, yGitHubLabel);

			}

			bmp.Save(pngFilePath, ImageFormat.Png); // can overwrite old png
		}

		// I need to review the constitution of contests:
		// (Does each voter see 1 Senate + 1 Congress)?
		// These would affect the divisor (2) for OTHER votes.

		// results: I see multiple Representatives per locality,
		// but they have different "District #" so I conclude
		// every voter sees one Representativ;
		// There is exactly 1 Senate race in Colorado.
		// So keeping the Divisor (2) to average cats.
		
		// Also for parity of REP:DEM in case some contest is unopposed!
		
		static void OutputGrainContestChoiceParty()
		{
			// oops, never saved names in each object!
			// Wake up! no need: name IS the dict key.
			foreach(KeyValuePair<string,Grain> kvp1 in grains)
			{
				say("");
				say("==========");
				say("locality: " + kvp1.Key);
				foreach(KeyValuePair<string,Contest> kvp2 in kvp1.Value.contests)
				{
					say("");
					say("contest: " + kvp2.Key);
					bool hasRep = false;
					bool hasDem = false;
					foreach(KeyValuePair<string,Choice> kvp3 in kvp2.Value.choices)
					{
						switch(kvp3.Value.party)
						{
							case "REP":
								hasRep = true;
								break;
							case "DEM":
								hasDem = true;
								break;
						}
						say("choice (" + kvp3.Value.party + "): " + kvp3.Key);
					}
					if( ! (hasRep && hasDem))
					say("*** UNCONTESTED: " + kvp2.Key);
				}
			}
		}
		
	}
}
