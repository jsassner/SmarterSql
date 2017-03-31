<h3><strong>Project Description</strong></h3>
<div class="wikidoc">An addin to Microsoft Sql Server Management Studio 2005 and 2008 which enables the user to use intellisense with added features.</div>
<p><span style="text-decoration: underline;"><strong>The code is in a pretty ok state function wise, but it's neither 100% complete and there are still bugs in it.</strong></span></p>
<p>I've stopped working on this addon over one year ago, and it was time for someone else to continue my work.</p>
<p>&nbsp;</p>
<div class="wikidoc">Also, check out my <a href="http://www.codeproject.com/KB/database/SSMSKeyBindings.aspx"> KeyBinding addin</a> on Code Project.</div>
<div class="wikidoc">&nbsp;</div>
<h3>Quick manual</h3>
<p>A window with tables, aliases, functions and SQL commands pops up when typing in text editor. In the example below, you press the point, and then got up the columns in the table. What is shown is the name of the column data type, the column can be null or not and as a tooltip on the far right appears from the table it will. What appears to the right of the list and what shows up as tooltip is dependent on the type of data displayed on the left.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248991"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image002" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248992" alt="clip_image002" width="553" height="106" border="0" /></a></p>
<p>&nbsp;</p>
<p>In the example below shows the tables in schema SalesLT. As a tooltip displays the contents of the active table.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248993"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image002[7]" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248994" alt="clip_image002[7]" width="608" height="157" border="0" /></a></p>
<p>To select a row in intellisense window, you can press the space, tab, enter, or get left parenthesis. The various keys have slightly different behavior. Tab and enter shooting only in a selected row, spaces, indents the selected row and type a space.</p>
<h3>&nbsp;</h3>
<h3>Help texts to functions</h3>
<p>Many sql functions has a help text that describes the parameters of the function has. The example below shows how the command LEFT () function. The top row shows the parameters that the function takes (the one in bold is the active parameter), and what the function returns (in this example a string). Next part describes briefly the function and the last paragraph describes the current parameter bit more detail.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248995"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image004" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248996" alt="clip_image004" width="577" height="139" border="0" /></a></p>
<h3>&nbsp;</h3>
<h3>Expand table columns</h3>
<p>If one of a select statement writes a table alias with trailing asterisk (*), a tooltip pops up after a while saying that pressing the Tab key expands all the columns of the table. In the example below lists the five columns from table Customer when pressing the Tab key.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248997"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image006" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248998" alt="clip_image006" width="322" height="85" border="0" /></a></p>
<h3>Expand the stored procedure parameters</h3>
<p>If a stored procedure is selected in a intellisense window, a tooltip pops up after a while saying that pressing the Tab key expands all parameters for the stored procedure. In the example below, the result uspLogError &lt;@ ErrorLogID int (4)&gt; after the Tab key is pressed on.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=248999"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image008" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249000" alt="clip_image008" width="294" height="98" border="0" /></a></p>
<h3>&nbsp;</h3>
<h3>Sql error</h3>
<p>SmarterSql is looking for a number of errors in the code written. It may be that such a schedule made ​​for a table in a FROM clause. As indicated with a wavy red line under the table name. Hover over the red line shows what is considered wrong. In the example below, the text would read: "No source table schema found".</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249001"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image010" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249002" alt="clip_image010" width="318" height="94" border="0" /></a></p>
<p>SmarterSql can help to fix the problem. If you put the cursor somewhere on the red line and press alt + enter, the window pops up in the picture above. In this case there was a schedule that was adopted, and if you press enter to add the proper schema to (in this case, the result FROM SalesLT.Customer AS c). Using the alt + F12 and alt + shift + F12 moves the cursor to the next or previous sql error. </p>
<h3>Multi insert column alias</h3>
<p>Sql error function can also be used to add column aliases on multiple columns simultaneously. The following figure is approximately the column alias AddressID and address type and the alias is put on table customer address.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249003"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image012" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249004" alt="clip_image012" width="335" height="126" border="0" /></a></p>
<h3>&nbsp;</h3>
<h3>Mouse over token</h3>
<p>When the mouse is over a table name, table alias, etc. displays a tooltip with information about the current object. In the example below, held the mouse over table customer address.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249005"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image014" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249006" alt="clip_image014" width="291" height="117" border="0" /></a></p>
<h3>&nbsp;</h3>
<h3>Highlight token usage</h3>
<p>You can right click on a table alias and select highlight usage in file (alternatively press Ctrl + Shift +7) to mark where the alias is used. Yellow marker is where it is defined and green where it is used.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249007"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image016" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249008" alt="clip_image016" width="608" height="236" border="0" /></a></p>
<p>&nbsp;</p>
<p>Via the menu (Goto and Goto previous next) or Ctrl + Alt + arrow up / down arrows, you can then move between declarations. <br /> Goto declaration </p>
<p>Menu options Goto declaration (alt press ctrl + b) moving the focus from the use of a table alias to it was declared. <br /> Rename Token </p>
<p>Menu options Rename Token (alt press Shift + F6) opens up a dialog where a new name can be entered. An OK button or Enter key rename alias table, and all the places it is used in the sqlsats where it is used.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249009"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image018" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249010" alt="clip_image018" width="225" height="108" border="0" /></a></p>
<h3>&nbsp;</h3>
<h3>Live templates</h3>
<p>There are a number of Live templates defined. It is little short commands that, when used, expanded to a larger text. Tex expanded "ssf" to "SELECT * FROM dbo.", and "dv" expands to "DECLARE @ str varchar ()". </p>
<h3>Ambiguous columns</h3>
<p>If a column does not have a nickname, and this column is available in two tables from the set, must be an alias added. Alt + Enter opens a menu of the table sources that can be added.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249011"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image020" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249012" alt="clip_image020" width="441" height="105" border="0" /></a></p>
<h3>&nbsp;</h3>
<h3>Expand insert columns</h3>
<p>If, after an insert command, type a parenthesis "(", a tooltip after a short while as saying that pressing the Tab key expands the columns in the table. In the example below lists the nine columns of Table error log when printed the Tab key.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249013"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image022" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249014" alt="clip_image022" width="408" height="42" border="0" /></a></p>
<p>&nbsp;</p>
<h3>Expand / minimize segments</h3>
<p>Select kits, update rates, etc may be a small minus sign to the left of the set as if you click the minimize / expand the kit. If you hold your mouse over the row that was left is shown the rate is hidden in the minimized set.</p>
<p><a href="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249015"><img style="padding-left: 0px; padding-right: 0px; display: inline; padding-top: 0px; border: 0px;" title="clip_image024" src="http://download.codeplex.com/download?ProjectName=smartersql&amp;DownloadId=249016" alt="clip_image024" width="191" height="86" border="0" /></a></p>
