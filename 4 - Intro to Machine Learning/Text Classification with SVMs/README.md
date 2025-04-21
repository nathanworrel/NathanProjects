# Project 1 – Text Classification with SVMs

## 📄 Overview
This project focuses on building a binary text classification system using **Support Vector Machines (SVMs)**. The primary goal is to classify text reviews as either **positive (1)** or **negative (-1)** using a bag-of-words representation and machine learning models.

## 🧠 Learning Objectives
- Preprocess and extract features from raw text data.
- Understand and implement **bag-of-words** representations.
- Train and evaluate **SVM classifiers**.
- Use **cross-validation** for model selection and tuning.
- Analyze performance metrics such as accuracy.

## 🧰 Tools & Libraries
- Python
- `pandas`, `numpy`
- `scikit-learn` (SVMs, cross-validation)

## 📁 Key Components
### 1. `extract_dictionary(df)`
- Extracts a vocabulary dictionary from the training dataset.
- Maps each unique word to a feature index.

### 2. `generate_feature_matrix(df, word_list)`
- Converts each document to a binary feature vector using the bag-of-words model.
- Uses the dictionary from `extract_dictionary`.

### 3. `train_and_predict_svm(X_train, y_train, X_val)`
- Trains a linear SVM classifier using the training set.
- Predicts labels for the validation/test set.

### 4. `cross_validation(X, y, k=5)`
- Performs k-fold cross-validation to evaluate model performance and select optimal hyperparameters.

## 📊 Evaluation Metrics
- **Accuracy**: Fraction of correctly classified examples.
- **Cross-validation score**: Helps avoid overfitting and find optimal SVM parameters.

## 📌 Dataset
- CSV format: contains columns such as `text` (review) and `label` (+1 or -1).
- Training and test splits used for model development and evaluation.

