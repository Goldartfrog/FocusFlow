import numpy as np
import matplotlib.pyplot as plt

# 读取数据
data = np.loadtxt('./CalibrationTest/0/4/depth.txt', delimiter=',')
vector1 = data[:, 1:4]  # 第一个向量
vector2 = data[:, 4:7]  # 第二个向量
startpoint1 = data[:, 10:13]  # 第一个向量的起始点
startpoint2 = data[:, 13:16]  # 第二个向量的起始点

# 创建一个平面图
plt.figure()

# 绘制向量
for i in range(len(data)):
    vector1_color = 'blue'
    vector2_color = 'red'
    
    # 绘制第一个向量的延伸线段
    plt.plot(
        [startpoint1[i, 0], startpoint1[i, 0] + 2*vector1[i, 0]],
        [startpoint1[i, 2], startpoint1[i, 2] + 2*vector1[i, 2]],
        color=vector1_color, label='Vector 1', alpha=0.1
    )
    
    # 绘制第二个向量的延伸线段
    plt.plot(
        [startpoint2[i, 0], startpoint2[i, 0] + 2*vector2[i, 0]],
        [startpoint2[i, 2], startpoint2[i, 2] + 2*vector2[i, 2]],
        color=vector2_color, label='Vector 2', alpha=0.1
    )

# 设置图像标签和标题
plt.xlabel('X')
plt.ylabel('Z') 
plt.xlim([-0.05, 0.05])
plt.ylim([-1, 5])
plt.title('Vector Extension Visualization in X-Z Plane')

# 显示图像
# plt.legend()
plt.savefig('visualize_4.png', dpi=300)
