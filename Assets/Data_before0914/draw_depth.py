import matplotlib.pyplot as plt

# data_file1 = 'depth_change_raw.txt'
# time1 = []
# depth1 = []
# with open(data_file1, 'r') as file:
#     for line in file:
#         parts = line.split(',')
#         time1.append(float(parts[0]))
#         depth1.append(float(parts[3]))

# data_file2 = 'depth_change_mean.txt'
# time2 = []
# depth2 = []
# with open(data_file2, 'r') as file:
#     for line in file:
#         parts = line.split(',')
#         time2.append(float(parts[0]))
#         depth2.append(float(parts[3]))

# plt.plot(time1, depth1, color='red', linewidth=0.5, label='Raw Data', alpha = 0.5)
# plt.plot(time2, depth2, color='blue', linewidth=0.5, label='Mean Data')
# plt.xlabel('Time')
# plt.ylabel('Depth')
# plt.grid(True)
# plt.legend()

# plt.axhline(y=1.2, color='red', linestyle='--')

# plt.savefig('depth_curves.png', dpi=300)

data_file = './Calibration/0/2/depth.txt'
time = []
depth1, depth2, depth3 = [], [], []
with open(data_file, 'r') as file:
    for line in file:
        parts = line.split(',')
        time.append(float(parts[0]))
        depth1.append(float(parts[7]))
        depth2.append(float(parts[8]))
        depth3.append(float(parts[9]))

plt.plot(time, depth1, color='red', linewidth=0.5, label='Raw Estimation')
plt.plot(time, depth2, color='blue', linewidth=0.5, label='Averaged Estimation')
plt.plot(time, depth3, color='green', linewidth=0.5, label='Ground Truth')
plt.xlabel('Time')
plt.ylabel('Depth')
plt.ylim([0, 2])
plt.grid(True)
plt.legend()

plt.savefig('depth_close_2.png', dpi=300)
