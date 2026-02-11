# A02WindowsNetwork
assignment 2 
Tasks no Threads

A02Report.pdf

Asynchronous using TCPListener and TcpClient classes.

Game the server provides a 30 string word that can be spelt backwards or forwards

must use aysync and await methods 



Client/Player
UI(WPF)
_____________________________

            title
            about

    start game      quit
_____________________________

            timer
sentence (slowly fills as game goes on)

   textbox          guess 
____________________________

Client Logic
task to act as timer to display to the user
Stores the sentence on the client

word search logic
    does the word exist in the string
    has the user guest it already
    display newly found words into the wpf

Failure when timer reaches zero

Logic
start game connects the client to the server.
config button to change ip and such

Four files

File #1
    Line 1: helpleasetsetablenjoyablendads
    Line 2: 16 words
    left to right
        help
        plea
        please
        set
        sets
        table
        len
        joy
        enjoyable
        enjoy
        able
        end
        blend
        dad
        dads
    right to left
        bat
        test

File #2
    Line 1: 
    Line 2:

File #3
    Line 1: 
    Line 2:

file #4
    Line 1: 
    Line 2:

GamePlay Flow
user presses start game
client connects to the server
server selects at random one of the 4 files
server sends number of words to the client
client disconnects from the server
client then guesses a word
client sends word to the server
server checks if its in the string or not.
gives the client a response depending on whether the word was inside or not
displays newly found words
client checks if full game is over
game ends
game asks to replay or end
client closes

Client Protocol (should use some sort of number key system to make protocols easier to use ex: guess = 200)
____________________________
        Request                 Login
        Protocol ID             200
        Client GUID             ASDFAJIHEFRJW-234lkihgba0sdf-234zsdf
        Time Sent               2024-23-23
        Action                  Login
        Action Data             username:password:ip:port
        END                     |END|

        Format:                 Protocol ID|Client GUID|Time Sent|Action|Action Data|END|
        Example:                200|ASDFAJIHEFRJW-234lkihgba0sdf-234zsdf|2024-23-23|Login|username:password:ip:port|END|
____________________________
        Request                 Guess
        Protocol ID             201
        Client GUID             ASDFAJIHEFRJW-234lkihgba0sdf-234zsdf
        Time Sent               2024-23-23
        Action                  Guess
        Action Data             Hello (this is the word they guessed)
        END                     |END|

        Format:                 Protocol ID|Client GUID|Time Sent|Action|Action Data|END|
        Example:                201|ASDFAJIHEFRJW-234lkihgba0sdf-234zsdf|2024-23-23|Guess|Hello|END|
____________________________
        Request                 New Game
        Protocol ID             202
        Client GUID             ASDFAJIHEFRJW-234lkihgba0sdf-234zsdf
        Time Sent               2024-23-23
        Action                  New Game
        Action Data             -
        END                     |END|

        Format:                 Protocol ID|Client GUID|Time Sent|Action|Action Data|END|
        Example:                202|ASDFAJIHEFRJW-234lkihgba0sdf-234zsdf|2024-23-23|New Game|-|END|
____________________________
        Request                 Quit Game
        Protocol ID             203
        Client GUID             ASDFAJIHEFRJW-234lkihgba0sdf-234zsdf
        Time Sent               2024-23-23
        Action                  Quit Game
        Action Data             -
        END                     |END|

        Format:                 Protocol ID|Client GUID|Time Sent|Action|Action Data|END|
        Example:                203|ASDFAJIHEFRJW-234lkihgba0sdf-234zsdf|2024-23-23|Quit Game|-|END|
____________________________
    Play Again:
      



      -Server sends: 
      server state Format 
      Ex: 
      PROTOCOL ID 200 
      SERVER STATE 200 
      MESSAGE explains server status 
      END (used to tell the reader they have read it all)


Server Protocol
    Timer Run out:
        -a game over of sorts
    Server Going down:
        -server being shut down for whatever reason

    

Server Statebag (what is stored to determine a game state for a certain user)

-Client Ip, Port
-GUID of user
-Number of guesses
-number of words found
    -maybe a list of found words
    -list of total words
    -the difference between them to determine how many more to guess
-Start Timer
-file user is playing on

CheckList
File structure
connection between client and server
    protocol
add client list stored on server side
server UI log
file loader
gameguesser
game state
time checker