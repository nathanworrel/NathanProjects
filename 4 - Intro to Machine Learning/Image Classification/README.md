# Project 2: Karl’s Convolutional Kennel 🐶

## 🧠 Overview
This project explores **deep learning techniques** for image classification with a focus on **convolutional neural networks (CNNs)**, **transfer learning**, and **data augmentation**. The task is to help Karl, a kennel owner, identify whether a given dog image is a **Golden Retriever or a Collie** using supervised learning.

---

## 📦 Dataset
- 12,775 images across **10 dog breeds**
- Each image: 64×64 RGB
- Data partitions: Train, Validation, Test, and Challenge
- Metadata provided in `dogs.csv`

---

## 🔧 Key Components

### 1. Data Preprocessing
- Normalize RGB images using channel-wise mean and standard deviation (from training set)
- Implemented in `dataset.py`
- Ensures input consistency for training

### 2. Convolutional Neural Networks (CNNs)
- Implemented a custom **Target CNN** for binary classification (Collie vs Golden Retriever)
- Architecture:
  - 3 convolutional layers
  - 2 max pooling layers
  - 1 fully connected output layer
- Early stopping using validation loss
- Evaluation: Accuracy, Loss, and **AUROC**

### 3. CNN Visualization
- Used **Grad-CAM** to visualize regions of the image contributing to model predictions
- Helped understand CNN focus on key features for classification

### 4. Transfer Learning & Data Augmentation

#### Transfer Learning
- Pretrained a model on 8 other dog breeds (source task)
- Transferred learned weights to target model
- Experimented with freezing different layers

#### Data Augmentation
- Techniques: **Random rotation**, **Grayscale conversion**
- Enhanced generalization by increasing data variability
- Only applied to training set

### 5. Challenge Task
- Designed a custom CNN architecture with:
  - Flexible filter sizes
  - Custom activation functions
  - Dropout & weight decay for regularization
  - Transfer learning and data augmentation
- Evaluated using **AUROC** on held-out challenge set

---

## 🛠 Tools & Libraries
- **Framework**: PyTorch
- **Packages**: 
  - `torch`, `torchvision`
  - `scikit-learn`, `matplotlib`
  - `numpy`, `pandas`, `imageio`, `pillow`, `scipy`

---

## 📊 Evaluation Metrics
- Accuracy
- Area Under ROC Curve (AUROC)
- Validation loss used for **early stopping**
- Final challenge model graded on:
  - AUROC
  - Effort and experimentation

---

## ✅ Deliverables
- Code appendix with full implementation
- Visualizations for CNN activations
- AUROC results for all major experiments
- `uniqname.csv` containing challenge predictions

