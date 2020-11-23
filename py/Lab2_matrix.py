import numpy as np
import numpy.linalg as LA

with open('matrix.txt', 'r') as f:
    matrix = []
    for line in f:
        arr = [el for el in map(int, line.split(','))]
        matrix.append(arr)

matrix = np.array(matrix)
eigen_values = LA.eig(matrix)[0]
result = 1 / np.size(matrix, 0)
for eigen_value in eigen_values:
    if not np.isclose(eigen_value, 0):
        result = eigen_value * result

print(f'Number of spanning trees is {int(round(result))}')
