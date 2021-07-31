import numpy as np

win = np.load('Agent_GitHub\Fight\\NFSP_DQN.npy')
win = list(win)
print(win)

# a = [np.array([0, 0, 0]), np.array([ 1,  0, 28]), np.array([ 1,  1, 39]), np.array([ 2,  1, 39]), np.array([ 3,  1, 39]), np.array([ 4,  1, 28]), np.array([4, 2, 1]), np.array([ 5,  2, 38]), np.array([ 6,  2, 37]), np.array([ 7,  2, 40]), np.array([ 7,  3, 29]), np.array([ 8,  3, 13]), np.array([ 8,  4, 15]), np.array([ 9,  4, 33]), np.array([10,  4, 39]), np.array([10,  5, 40]), np.array([10,  6, 39]), np.array([11,  6,  5]), np.array([12,  6, 33]), np.array([12,  7, 39]), np.array([13,  7, 40]), np.array([14,  7, 39]), np.array([14,  9, 10]), np.array([14, 10, 38]), np.array([14, 11, 39]), np.array([15, 11, 40]), np.array([15, 12, 35]), np.array([16, 12, 38]), np.array([16, 13, 30]), np.array([17, 13, 28]), np.array([17, 14, 40]), np.array([18, 14, 40]),np.array([19, 14, 18]), np.array([19, 15, 40]), np.array([20, 15, 34]), np.array([21, 15, 39]), np.array([21, 16, 39]), np.array([22, 16, 22]), np.array([23, 16, 10]), np.array([24, 16, 40]), np.array([25, 16, 38]), np.array([25, 17, 28]), np.array([25, 18, 36]), np.array([25, 19,  4]), np.array([26, 19, 34])]

# b = [np.array([0, 1, 7]), np.array([ 1,  1, 40]), np.array([ 2,  1, 38]), np.array([ 2,  2, 36]), np.array([ 2,  3, 40]), np.array([ 3,  3, 31]), np.array([ 4,  3, 28]), np.array([ 4,  4, 39]), np.array([ 4,  5, 25]), np.array([ 4,  6, 22]), np.array([ 5,  6, 39]), np.array([ 5,  7, 39]), np.array([ 5,  8, 40]), np.array([ 5,  9, 40]), np.array([ 6,  9, 38]), np.array([ 7,  9, 26]), np.array([ 7, 10, 40]), np.array([ 8, 10, 38]), np.array([ 9, 10, 38]), np.array([10, 10, 40]), np.array([11, 10, 40]), np.array([12, 10, 39]), np.array([12, 11, 19]), np.array([13, 11, 38]), np.array([14, 11, 34]), np.array([14, 12, 37]), np.array([15, 12, 34]), np.array([15, 13, 40]), np.array([15, 14, 36]), np.array([16, 14, 24]), np.array([17, 14, 39]), np.array([18, 14, 30]), np.array([19, 14, 29]), np.array([20, 14, 30])]

# x = a[-1]
# lst = []
# print(x)
# for i in b:
#     lst.append(np.array([x[0]+i[0],x[1]+i[1],i[2]]))
# a.extend(lst)
# np.save('Agent_GitHub\Fight\\NFSP_DQN.npy',a)
# print(a)
