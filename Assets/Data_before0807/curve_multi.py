import matplotlib.pyplot as plt
import numpy as np

def mapping(values):
    for i in range(len(values)):
        if values[i] <= 1:
            continue
        elif 1 < values[i] <= 1.5:
            values[i] = 1 + (values[i] - 1) * 2
        else:
            values[i] = 2 + (values[i] - 1.5) * 2
    return values

def plot_density(data_file, ax):
    time = []
    depth1, depth2, depth3 = [], [], []
    with open(data_file, 'r') as file:
        for line in file:
            parts = line.split(',')
            time.append(float(parts[0]))
            depth1.append(float(parts[7]))
            depth2.append(float(parts[8]))
            depth3.append(float(parts[9]))

    ax.plot(time, depth1, color='red', linewidth=0.5, label='Raw Estimation')
    ax.plot(time, depth2, color='blue', linewidth=0.5, label='Averaged Estimation')
    ax.plot(time, depth3, color='green', linewidth=0.5, label='Ground Truth')
    ax.grid(True)
    # ax.legend(fontsize=5)

# 数据文件列表
data_files = ['./CalibrationTest/0/9/depth.txt', './CalibrationTest/1/9/depth.txt', './CalibrationTest/4/7/depth.txt']  # 假设有3个数据文件

# 每个子图的高度
num_plots = len(data_files)
subplot_height = 1
fig, axs = plt.subplots(num_plots, 1, figsize=(8, subplot_height * num_plots + 1), sharex=True)

# 绘制每个子图
for i, (data_file, ax) in enumerate(zip(data_files, axs)):
    ax.set_xlim([0, 35])  # 可根据需要设置x轴范围
    ax.set_ylim([0, 5])  # 可根据需要设置y轴范围
    plot_density(data_file, ax)

    if i == num_plots - 1:
        # 在最后一个子图上显示刻度
        ax.set_xlabel('Time(s)')
        
        ax.tick_params(axis='x', labelbottom=True)
        
    # # 显示每个子图的纵轴刻度
    ax.tick_params(axis='y', labelleft=True)

plt.figtext(0.07, 0.475, 'Depth(m)', rotation=90, va='center')
plt.figtext(0.03, 0.75, 'P1', rotation=0, va='center')
plt.figtext(0.03, 0.475, 'P2', rotation=0, va='center')
plt.figtext(0.03, 0.2, 'P3', rotation=0, va='center')

# 调整子图之间的间距
plt.subplots_adjust(hspace=0.3)

plt.savefig('depth_3.png', dpi=300)
