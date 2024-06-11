import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.svm import SVR
from sklearn.metrics import mean_squared_error
import pickle
import matplotlib.pyplot as plt

error_averaged = []
error_predicted = []
iterations = 5
res = 0.3

file_pre = 'calibration_test_'
file_suf = '.txt'

for i in range(1, iterations+1):
    file_name = file_pre + str(i) + file_suf
    # Load data from file
    data = np.loadtxt(file_name, delimiter=',')
    prev_feature = [0] * 350
    features, targets = [], []

    for j in range(len(data)):
        prev_feature = prev_feature[7:]
        tmp = data[j][1:7].tolist()
        tmp.append(data[j][8])
        prev_feature.extend(tmp)
        if j >= 49:
            features.append(prev_feature)
            targets.append(data[j][9])

    features = np.array(features)
    targets = np.array(targets)

    with open('svm_regression_model_tiansu_1.pkl', 'rb') as file:
        model = pickle.load(file)

    predictions = model.predict(features)

    avg_depth = features[:, -1]
    avg_mse = mean_squared_error(targets, avg_depth)
    print(f"Avg Mean Squared Error: {avg_mse}")

    # Evaluate the model
    mse = mean_squared_error(targets, predictions)
    print(f"Mean Squared Error: {mse}")

    time = data[49:, 0]
    plt.figure(figsize=(10,6))
    plt.plot(time, targets, color='green', linewidth=0.5, label='Ground Truth')
    plt.plot(time, features[:, -1], color='red', linewidth=0.5, label='Averaged Estimation', alpha=0.5)
    plt.plot(time, model.predict(features), color='blue', linewidth=0.5, label='SVR Prediction', alpha=0.5)
    plt.ylim([0, 10])
    plt.xlabel('Time')
    plt.ylabel('Depth')
    plt.grid(True)
    plt.legend()

    plt.savefig("depth_change_" + str(i) + ".png", dpi=300)

    avg_depths = features[:, -1]
    pred_depths = predictions
    targets = targets

    error_count_averaged = 0
    error_count_predicted = 0
    total_count = len(targets)

    for k in range(len(targets)):
        if abs(targets[k]-1) <= 0.2:
            if avg_depths[k] > 1 + res:
                error_count_averaged += 1
            if pred_depths[k] > 1 + res:
                error_count_predicted += 1
        else:
            if avg_depths[k] <= 1 + res:
                error_count_averaged += 1
            if pred_depths[k] <= 1 + res:
                error_count_predicted += 1
    
    error_averaged.append(error_count_averaged / total_count)
    error_predicted.append(error_count_predicted / total_count)

plt.figure(figsize=(10,6))
x = range(iterations)

bar1 = plt.bar([i - 0.2 for i in x], height = error_averaged, width = 0.4, alpha = 0.8, color = 'r',label = 'Averaged Depth Error Rate')

bar2 = plt.bar([i + 0.2 for i in x], height = error_predicted, width = 0.4, alpha = 0.8, color = 'g', label = 'Prediction Depth Error Rate')

xticks = ["%d" %(x) for x in np.arange(1, 1+iterations)]
xticks[0] = "1.5"

plt.xticks(x, xticks)
plt.ylim(0, 1)
plt.xlabel('distance')
plt.ylabel('error rate')
plt.legend() 

for rect in bar1:
    height = rect.get_height()
    plt.text(rect.get_x() + rect.get_width() / 2, height+0.01, str(height)[:4], ha="center", va="bottom")
for rect in bar2:
    height = rect.get_height()
    plt.text(rect.get_x() + rect.get_width() / 2, height+0.01, str(height)[:4], ha="center", va="bottom")

plt.savefig('error_rate_1.png', dpi=300)