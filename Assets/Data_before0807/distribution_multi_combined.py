import matplotlib.pyplot as plt
import numpy as np

colors = ['tab:blue', 'tab:orange', 'tab:green']

def mapping(values):
    for i in range(len(values)):
        if values[i] <= 1:
            continue
        elif 1 < values[i] <= 1.5:
            values[i] = 1 + (values[i] - 1) * 2
        else:
            values[i] = 2 + (values[i] - 1.5) * 2
    return values

def plot_density(data_file, axis_type, ax):
    for index in range(len(data_file)):
        data = np.loadtxt(data_file[index], delimiter=',', skiprows=1)

        if axis_type == 'raw':
            depth = data[:, 7]  # 使用raw_depth
            title = 'Raw Depth'
        elif axis_type == 'average':
            depth = data[:, 8]  # 使用averaged_depth
            title = 'Averaged Depth'
        else:
            raise ValueError("Invalid axis_type. Must be 'raw' or 'average'.")

        for i in range(len(depth)):
            if abs(data[i][-1] - 2) > abs(data[i][-1] - 0.5):
                depth[i] -= data[i][-1] - 0.5
            else:
                depth[i] -= data[i][-1] - 2

        # depth = mapping(depth)

        ax.hist(depth, bins=500, density=True, color=colors[index])


# 数据文件列表
data_files = [['./CalibrationTest/0/2/depth.txt', './CalibrationTest/0/4/depth.txt'], ['./CalibrationTest/1/1/depth.txt', './CalibrationTest/1/5/depth.txt'], ['./CalibrationTest/4/2/depth.txt', './CalibrationTest/4/6/depth.txt']]  # 假设有3个数据文件

# 每个子图的高度
num_plots = len(data_files)
subplot_height = 1
fig, axs = plt.subplots(num_plots, 1, figsize=(subplot_height * num_plots + 1, subplot_height * num_plots + 1), sharex=True)

# 绘制每个子图
for i, (data_file, ax) in enumerate(zip(data_files, axs)):
    ax.set_xlim([0, 5])  # 可根据需要设置x轴范围
    ax.set_ylim([0, 5])  # 可根据需要设置y轴范围
    plot_density(data_file, 'raw', ax)  # 绘制raw_depth概率密度图，如需绘制averaged_depth，将'raw'改为'average'

    if i == num_plots - 1:
        # 在最后一个子图上显示刻度
        ax.set_xlabel('Depth(m)')
        
        ax.tick_params(axis='x', labelbottom=True)
        
    # # 显示每个子图的纵轴刻度
    ax.tick_params(axis='y', labelleft=False)

plt.figtext(0.07, 0.475, 'Frequency', rotation=90, va='center')
plt.figtext(0.03, 0.75, 'P1', rotation=0, va='center')
plt.figtext(0.03, 0.475, 'P2', rotation=0, va='center')
plt.figtext(0.03, 0.2, 'P3', rotation=0, va='center')

# 调整子图之间的间距
plt.subplots_adjust(hspace=0.3)

plt.savefig('depth_test.png', dpi=300)
