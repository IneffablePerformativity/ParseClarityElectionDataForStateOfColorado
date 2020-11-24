# ParseClarityElectionDataForStateOfColorado
Yet another parser of Clarity Election Data, this time for Colorado. Biden Bonus = +3%. Trump Bonus = -1%.

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
 ...
 
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
...
