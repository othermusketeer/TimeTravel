TimeTravel
=====

Author: D'Artagnan Palmer  
StackOverflow User: <https://stackoverflow.com/users/3453155/othermusketeer>  
SuperUser User: <https://superuser.com/users/332891/othermusketeer>  
Date Start: 2018-02-12  
Date Updated: 2018-02-12  
Copyright: Public Domain  

About
-----
This program was inspired by a [question on SuperUser.com](https://superuser.com/questions/1294289/how-to-stop-win10-from-correcting-the-date-time-automatically), it allows you to force the date and time every second.
When the program closes/ends, it will restore the previous date and time. You can specify a date,
a date and time, or an offset from the current time. There is much room for improvement of program,
but I will not bother unless someone asks. This program requires administrator rights in order to
change the time. Please RunAs Administrator.

Important
---------
This program requires administrator rights in order to change the time. Please RunAs
Administrator.

Usage
-----
`TimeTravel\t<YYYY>(/|.|-)<MM>(/|.|-)<DD>`

`TimeTravel\t<YYYY>(/|.|-)<MM>(/|.|-)<DD>( |.|-)<hh>(:|.|-)<mm>(:|.|-)<ss>`

`TimeTravel\t(+|-)<number>(Y|M|D)`

Examples
--------
`TimeTravel 2014-04-22`

`TimeTravel 2012/01/30 22:30:04`

`TimeTravel 2017.9.7.1.2.3`

`TimeTravel 5d`

`TimeTravel +3y`

`TimeTravel -7m`