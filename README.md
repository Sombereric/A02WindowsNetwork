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
Line 1: helpleasetsetablenjoyablendad
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
right to left
    bat
    test

File #2


File #3


file #4


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
    Server Online: 
        -Client logins
        -this pings the server to check if the server is online or not.
    Guess:
        -client sends a word to the server as a gues-
        -server must respond with a correct guess or wrong guess with updated list of all guessed words and timer
    New Game:
        -client presses new game
        -tells the server to create a new game state and adds to the list
        -server responses with a code to tell the client if the server succeeded in creating the game state
    Quit Game:
        -users quits or hits the exit button in the wpf
        -tells the server to delete the state session stored into a list
        -server deletes state and responses to the client with the successful deletion and removes ip and port from list as well
    Play Again:
        -user selects play again
        -replaces the stat with the new file
        -server sends the newly sent up game state

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

Example Game:

30 character string: Anyonextsetesnow

words from left to right
    a
    any
    anyone
    one
    next
    set
    now
    snow
    yo
words from right to left
    won
    test
    no