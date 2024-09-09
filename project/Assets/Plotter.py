import pandas as pd
import matplotlib.pyplot as plt

# Read the CSV file
df = pd.read_csv('../data.csv')

# Extract data from columns
t = df['t']
x_left = df['x(t) cubeL']
F_left = df['F(t) cubeL']
x_right = df['x(t) cubeR']
v_right = df['v(t) cubeR']
F_right = df['F(t) cubeR']
L_yellowTotal = df['L(y-axis) yellowL']
w_left = df['w(t) cubeL']
v_left = df['v(t) cubeL']

# Calculate min and max points
x_left_min, x_left_max = min(x_left), max(x_left)
F_left_min, F_left_max = min(F_left), max(F_left)
x_right_min, x_right_max = min(x_right), max(x_right)
v_right_min, v_right_max = min(v_right), max(v_right)
F_right_min, F_right_max = min(F_right), max(F_right)
L_yellowTotal_min, L_yellowTotal_max = min(L_yellowTotal), max(L_yellowTotal)
w_left_min, w_left_max = min(w_left), max(w_left)
v_left_min, v_left_max = min(v_left), max(v_left)

# Plotting x(t)
plt.figure(figsize=(10, 6))

plt.plot(t, x_left, label='x(t) Würfel 2', color='red')
plt.plot(t, x_right, label='x(t) Würfel 1', color='blue')

plt.xlabel('Zeit')
plt.ylabel('Position (x)')
plt.title('Positionsvergleich (Würfel 2 vs Würfel 1)')
plt.legend()

plt.text(t.iloc[x_left.idxmin()], x_left_min, f'Min: {x_left_min}', color='red')
plt.text(t.iloc[x_left.idxmax()], x_left_max, f'Max: {x_left_max}', color='red')
plt.text(t.iloc[x_right.idxmin()], x_right_min, f'Min: {x_right_min}', color='blue')
plt.text(t.iloc[x_right.idxmax()], x_right_max, f'Max: {x_right_max}', color='blue')

plt.savefig('position_comparison.png')
plt.close()

# Plotting v(t)
plt.figure(figsize=(10, 6))

plt.plot(t, v_right, label='v(t) Würfel 1', color='blue')

plt.xlabel('Zeit')
plt.ylabel('Geschwindigkeit (v)')
plt.title('Geschwindigkeitsvergleich (Würfel 2 vs Würfel 1)')
plt.legend()

plt.text(t.iloc[v_right.idxmin()], v_right_min, f'Min: {v_right_min}', color='blue')
plt.text(t.iloc[v_right.idxmax()], v_right_max, f'Max: {v_right_max}', color='blue')

plt.savefig('velocity_comparison.png')
plt.close()

# Plotting F(t)
plt.figure(figsize=(10, 6))

plt.plot(t, F_left, label='F(t) Würfel 2', color='red')
plt.plot(t, F_right, label='F(t) Würfel 1', color='blue')

plt.xlabel('Zeit')
plt.ylabel('Kraft (F)')
plt.title('Kraftvergleich (Würfel 2 vs Würfel 1)')
plt.legend()

plt.text(t.iloc[F_left.idxmin()], F_left_min, f'Min: {F_left_min}', color='red')
plt.text(t.iloc[F_left.idxmax()], F_left_max, f'Max: {F_left_max}', color='red')
plt.text(t.iloc[F_right.idxmin()], F_right_min, f'Min: {F_right_min}', color='blue')
plt.text(t.iloc[F_right.idxmax()], F_right_max, f'Max: {F_right_max}', color='blue')

plt.savefig('force_comparison.png')


# Plotting Angular Momentum Total, GLide and Rotate
plt.figure(figsize=(10, 6))

plt.plot(t, L_yellowTotal, label='L_Total(z-axis) gelbe Würfel', color='red')

plt.xlabel('Zeit')
plt.ylabel('SI: (kg * m^2 * s)')
plt.title('Drehimpulse des gelben Würfels (L) um die y-Achse')
plt.legend()

plt.text(t.iloc[L_yellowTotal.idxmin()], L_yellowTotal_min, f'Min: {L_yellowTotal_min}', color='red')
plt.text(t.iloc[L_yellowTotal.idxmax()], L_yellowTotal_max, f'Max: {L_yellowTotal_max}', color='red')

plt.savefig('angular_momentum_yellow.png')


# Plotting Velocity of the left cube
plt.figure(figsize=(10, 6))

plt.plot(t, w_left, label='w(t) cube links (y-axis)', color='green')

plt.xlabel('Zeit')
plt.ylabel('SI: 1/s')
plt.title('Winkelgeschwindigkeit (y-Achse)')
plt.legend()

plt.text(t.iloc[w_left.idxmin()], w_left_min, f'Min: {w_left_min}', color='green')
plt.text(t.iloc[w_left.idxmax()], w_left_max, f'Max: {w_left_max}', color='green')

plt.savefig('angular_velocity_cube_left.png')

# Plotting Velocity of the left cube
plt.figure(figsize=(10, 6))

plt.plot(t, v_left, label='v(t) cube links (x-axis)', color='blue')

plt.xlabel('Zeit')
plt.ylabel('m/s')
plt.title('Geschwindigkeit des linken Würfels (x-Achse)')
plt.legend()

plt.text(t.iloc[v_left.idxmin()], v_left_min, f'Min: {v_left_min}', color='blue')
plt.text(t.iloc[v_left.idxmax()], v_left_max, f'Max: {v_left_max}', color='blue')

plt.savefig('velocity_cube_left.png')

plt.close()
