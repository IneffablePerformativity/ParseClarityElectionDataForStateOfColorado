# ParseClarityElectionDataForStateOfColorado

Just click the file "AnalysisOf...-tiny.png" to view it in GitHub.

BIDEN BONUS = PLUS 1.3%, StdDev 1.04; TRUMP BONUS = MINUS 0.63%, StdDev 1.38

THE BLACK X'S (SORTED TO THE RIGHT) MARK 8 BIGGEST SPOTS OF THEFT OVER 4% WORTH 85249 VOTES. SEE LOG FOR LOCALITY NAMES.


/*
 * ParseClarityElectionDataForStateOfColorado.cs
 *
 * which code and results I will archive at:
 * https://github.com/IneffablePerformativity
 * https://github.com/IneffablePerformativity/ParseClarityElectionDataForStateOfColorado
 * 
 * 
 * "To the extent possible under law, Ineffable Performativity has waived all copyright and related or neighboring rights to
 * The C# program ParseClarityElectionDataForStateOfColorado.cs and resultant outputs.
 * This work is published from: United States."
 * 
 * This work is offered as per license: http://creativecommons.org/publicdomain/zero/1.0/
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
 * There are two app-generated conclusions:
 *
 * 1. THE BIDEN BONUS = PLUS 1.3%, StdDev 1.04; TRUMP BONUS = MINUS 0.63%, StdDev 1.38.
 *
 * 2. THE BLACK X'S (SORTED TO THE RIGHT) MARK 8 BIGGEST SPOTS OF THEFT OVER 4% WORTH 85249 VOTES. SEE LOG FOR LOCALITY NAMES.
 * 
 * 
 * There is a series of similar programs building upon one another,
 * named similarly, with XML inputs, HTML inputs, PDF-as-TXT input,
 * and some ugly earlier versions, all to be found at GitHub, here:
 * https://github.com/IneffablePerformativity
 * 
 * 
 * Pre- 2020-11-26 programs had an error, just fixed, soon to fix in those.
 * To Wit, I did not make a "Four" thing upon each grain, but outside loop.
 * Therefore, all the vote counts were merging into forever ascending sums.
 * Corrected on 2020-11-26. Henceforth you can trust me, 'til next mistake.
 * 
 * Pre- 2020-11-30 programs had an overflow error, just fixed in Colorado.
 * 
 * also note, I manually compress final image at tinypng.com
 */
