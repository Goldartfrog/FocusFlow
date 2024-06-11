import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.svm import SVR
from sklearn.metrics import mean_squared_error
import pickle
import matplotlib.pyplot as plt

# Load data from file
data = np.loadtxt('calibration_3.txt', delimiter=',')
prev_feature = [0] * 350
features, targets = [], []

for i in range(len(data)):
    prev_feature = prev_feature[7:]
    tmp = data[i][1:7].tolist()
    tmp.append(data[i][8])
    prev_feature.extend(tmp)
    if i >= 49:
        features.append(prev_feature)
        targets.append(data[i][9])

features = np.array(features)
targets = np.array(targets)

# Split data into training and testing sets
train_features, test_features, train_targets, test_targets = train_test_split(features, targets, test_size=0.2, random_state=42)

# Create and train the SVM regression model with RBF kernel
model = SVR(kernel='rbf', C=1.0, epsilon=0.2)
model.fit(train_features, train_targets)

# Make predictions on the test set
predictions = model.predict(test_features)

avg_depth = test_features[:, -1]
avg_mse = mean_squared_error(test_targets, avg_depth)
print(f"Avg Mean Squared Error: {avg_mse}")

# Evaluate the model
mse = mean_squared_error(test_targets, predictions)
print(f"Mean Squared Error: {mse}")

time = data[49:, 0]
plt.plot(time, targets, color='green', linewidth=0.5, label='Ground Truth')
plt.plot(time, features[:, -1], color='red', linewidth=0.5, label='Averaged Estimation', alpha=0.5)
plt.plot(time, model.predict(features), color='blue', linewidth=0.5, label='SVR Prediction', alpha=0.5)
plt.ylim([0, 10])
plt.xlabel('Time')
plt.ylabel('Depth')
plt.grid(True)
plt.legend()

plt.savefig('calibration_3.png', dpi=300)

# Save the trained model to a file
with open('svm_regression_model_tiansu_3.pkl', 'wb') as file:
    pickle.dump(model, file)