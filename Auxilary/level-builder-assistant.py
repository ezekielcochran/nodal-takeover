import pygame # type: ignore
import math

# Initialize Pygame
pygame.init()

# Set the dimensions of the window
window_width = 900
window_height = 600
CLICK_THRESHOLD = 5

# Create the window
window = pygame.display.set_mode((window_width, window_height))
click_positions = []
connection_positions = []

# Set the title of the window
pygame.display.set_caption("Click Coordinate Tracker")

def nearest_click(pos):
    min_dist = float('inf')
    nearest = None
    for click in click_positions:
        dist = math.sqrt((pos[0]-click[0])**2 + (pos[1]-click[1])**2)
        if dist < min_dist:
            min_dist = dist
            nearest = click
    return nearest

# Game loop
running = True
while running:
    # Handle events
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.MOUSEBUTTONDOWN:
            # Get the coordinates of the mouse click
            x, y = pygame.mouse.get_pos()
        elif event.type == pygame.MOUSEBUTTONUP:
            # Get the coordinates of the mouse release
            x2, y2 = pygame.mouse.get_pos()

            if (math.sqrt((x-x2)**2 + (y-y2)**2) < CLICK_THRESHOLD):
                # if the mouse is clicked and released at the same position, add the position to the list
                print(f"Clicked at ({x2}, {y2})")
                click_positions.append((x2, y2))
                # Draw a small circle at the clicked position
                pygame.draw.circle(window, (255, 0, 0), (x2, y2), 15)
                font = pygame.font.Font(None, 20)
                text = font.render(str(len(click_positions)), True, (255, 255, 255))
                text_rect = text.get_rect(center=(x2, y2))
                window.blit(text, text_rect)
            else:
                if len(click_positions) < 2:
                    print("Please add at least 2 nodes before adding connections")
                    continue
                (p1, p2) = nearest_click((x, y))
                (p3, p4) = nearest_click((x2, y2))
                # Draw a line between the two positions
                if ((p1, p2) != (p3, p4)):
                    pygame.draw.line(window, (0, 0, 255), (p1, p2), (p3, p4), 2)
                    connection_positions.append((p1, p2, p3, p4))
                # Update the display
                pygame.display.flip()
                

    # Update the display
    pygame.display.flip()

# Quit Pygame
pygame.quit()

click_positions.sort(key=lambda pos: pos[0])
node_positions = [(click[0] / window_width, (window_height - click[1])/window_height) for click in click_positions]
# Prune duplicates in connection_positions
unique_connections = set(connection_positions)
connection_positions = list(unique_connections)

def connection_to_indices(connection):
    p1, p2, p3, p4 = connection
    i1 = click_positions.index((p1, p2))
    i2 = click_positions.index((p3, p4))
    return i1, i2

# print results
print(f"\nnc {len(node_positions)}\n")
for node in node_positions:
    print(f"n {node_positions.index(node) + 1} {node[0]} {node[1]}")

print("")
for connection in connection_positions:
    i1, i2 = connection_to_indices(connection)
    i1, i2 = sorted([i1, i2])
    print(f"c {i1 + 1} {i2 + 1}")