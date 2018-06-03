Original By: https://github.com/ApokPT/Rocket-Plugin-Wrecking-Ball
Forked From: https://github.com/cartman-2000/Rocket-Plugin-Wrecking-Ball

# Wrecking Ball
### Destroy stuff in the server to clear lag
This addon allows you clear stuff in a defined radius using filters


## Available Commands
Command | Action
------- | -------
/wreck [steamId] <filter> <radius>				| Destroys filtered objects in radius
/wreck scan [steamId] <filter> <radius>			| Scans for filtered objects in radius
/wreck teleport <barricade|structure|vehicle>	| Teleports to random filtered object
/wreck list vehicles <radius>					| Lists vehicles and their barricades in radius
/wreck list topplayers [x=5]					| Lists x players with top object counts
/wreck confirm									| Confirms last destruction
/wreck abort									| Aborts last destruction


## Available Filters
Filter | Element
------- | -------
b				| Bed
t				| Trap
d				| Door
c				| Container
l				| Ladder
w				| Wall / Window
p				| Pillar
r				| Roof / Hole
s				| Stair / Ramp
m				| Freeform Buildables
n				| Signs
g				| Guards (Barricades / Fortifications)
i				| Illumination / Generators / Fireplaces
a				| Agriculture (plantations / platers)
v				| Vehicles
\*				| Everything except Zombies
z				| Zombies (killing too many zombies at once, crashes the server)
