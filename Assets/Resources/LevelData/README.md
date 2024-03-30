# Format of Level Data Files

Each level is defined by a .txt file, which contains information about node positions, connections, etc.

Each line of each level's file corresponds to one of the following:

| Identifier | Name | Data Type | Data |
| ------ | ------ | ------ | ------ |
| **ld** | Level Description | `string` | description of the level |
| **nc** | Node Count | `int` | total number of nodes |
| **n** | Node | `int float float` | node index, node x-coordinate, node y-coordinate |
| **c** | Connection | `int int` | The indexes of two connected nodes, with the first smaller than the second |

How to format:
- The lines should be organized into sections
  - One section for each type
  - Ordered as in the above table
- List nodes indices by increasing order, starting at 1
- List connections lexigraphically by endpoint node indices

### An example of proper formatting
```
ld A fun level!

nc 6

n 1 0.2 0.5
n 2 0.4 0.3
n 3 0.4 0.7
n 4 0.6 0.3
n 5 0.6 0.7
n 6 0.8 0.5

c 1 2
c 1 3
c 2 3
c 2 4
c 3 4
c 3 5
c 4 5
c 4 6
c 5 6
```

Node positions are given in a coordinate system from 0 to 1, with the orgin in the bottom left of the screen. (viewport coordinates)
Conventionally, list nodes in increasing index order, starting from 1 and moving up. **Missing indices may cause errors: ==the game scripts do not handle errors when parsing these files==.**