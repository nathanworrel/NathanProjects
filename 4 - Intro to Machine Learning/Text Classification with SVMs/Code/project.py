

from tkinter import Y
from turtle import pen
import pandas as pd
import numpy as np
import itertools
import string
import matplotlib as mpl

from sklearn.svm import SVC, LinearSVC
from sklearn.model_selection import StratifiedKFold
from sklearn import metrics
from matplotlib import pyplot as plt
import sklearn as sk


from helper import *

import warnings
from sklearn.exceptions import ConvergenceWarning

warnings.simplefilter(action="ignore", category=FutureWarning)
warnings.simplefilter(action="ignore", category=ConvergenceWarning)

np.random.seed(445)

def extract_word(input_string):
    """Preprocess review into list of tokens.

    Convert input string to lowercase, replace punctuation with spaces, and split along whitespace.
    Return the resulting array.

    E.g.
    > extract_word("I love it. It's my favorite course!")
    > ["i", "love", "it", "it", "s", "my", "favorite", "course"]

    Input:
        input_string: text for a single review
    Returns:
        a list of words, extracted and preprocessed according to the directions
        above.
    """
    # TODO: Implement this function - done
    hold = input_string
    hold = hold.lower()
    for ele in string.punctuation:
        hold = hold.replace(ele, " ")
    return hold.split()
    arr = hold.split()
    temp1 = []
    for i in range(len(arr)-1):
        temp1.append(arr[i] + " " + arr[i+1])
    temp2 = []
    for i in range(len(arr)-2):
        temp2.append(arr[i] + " " + arr[i+1] + " " + arr[i+2])
    last = np.concatenate([temp1, temp2, arr])
    return last
            


def extract_dictionary(df):
    """Map words to index.

    Reads a pandas dataframe, and returns a dictionary of distinct words
    mapping from each distinct word to its index (ordered by when it was
    found).

    E.g., with input:
        | text                          | label | ... |
        | It was the best of times.     |  1    | ... |
        | It was the blurst of times.   | -1    | ... |

    The output should be a dictionary of indices ordered by first occurence in
    the entire dataset:
        {
           it: 0,
           was: 1,
           the: 2,
           best: 3,
           of: 4,
           times: 5,
           blurst: 6
        }
    The index should be autoincrementing, starting at 0.

    Input:
        df: dataframe/output of load_data()
    Returns:
        a dictionary mapping words to an index
    """
    word_dict = {}
    # TODO: Implement this function - need to test
    spot = 0
    
    for info in df["text"].values:
        hold = extract_word(info)
        for word in hold:
            if word not in word_dict:
                word_dict.update({word: spot})
                spot = spot + 1
    return word_dict


def generate_feature_matrix(df, word_dict):
    """Create matrix of feature vectors for dataset.

    Reads a dataframe and the dictionary of unique words to generate a matrix
    of {1, 0} feature vectors for each review.  Use the word_dict to find the
    correct index to set to 1 for each place in the feature vector. The
    resulting feature matrix should be of dimension (# of reviews, # of words
    in dictionary).

    Input:
        df: dataframe that has the text and labels
        word_dict: dictionary of words mapping to indices
    Returns:
        a numpy matrix of dimension (# of reviews, # of words in dictionary)
    """
    number_of_reviews = df.shape[0]
    number_of_words = len(word_dict)
    feature_matrix = np.zeros((number_of_reviews, number_of_words))
    # TODO: Implement this function
    num = 0
    for ele in df["text"]:
        hold = extract_word(ele)
        for word in hold:
            feature_matrix[num, word_dict.get(word)] = 1
        num = num +1
    return feature_matrix




def performance(y_true, y_pred, metric="accuracy"):
    """Calculate performance metrics.

    Performance metrics are evaluated on the true labels y_true versus the
    predicted labels y_pred.

    Input:
        y_true: (n,) array containing known labels
        y_pred: (n,) array containing predicted scores
        metric: string specifying the performance metric (default='accuracy'
                 other options: 'f1-score', 'auroc', 'precision', 'sensitivity',
                 and 'specificity')
    Returns:
        the performance as an np.float64
    """
    # TODO: Implement this function
    # This is an optional but very useful function to implement.
    # See the sklearn.metrics documentation for pointers on how to implement
    # the requested metrics.
    if metric == "accuracy":
        return metrics.accuracy_score(y_true, y_pred)
    if metric == "auroc":
        return metrics.roc_auc_score(y_true, y_pred)

    tn, fp, fn, tp = metrics.confusion_matrix(y_true, y_pred).ravel()
    if metric == "precision":
        return tp / (tp + fp)
    elif metric == "sensitivity":
        return tp/(tp+fn)
    elif metric == "specificity":
        return tn/(tn+fp)
    elif metric == "f1-score":
        pr=tp / (tp+fp)
        se=tp/(tp+fn)
        return 2*pr*se/(pr+se)
    else:
        return metrics.accuracy_score(y_true, y_pred)
        


def cv_performance(clf, X, y, k=5, metric="accuracy"):
    """Split data into k folds and run cross-validation.

    Splits the data X and the labels y into k-folds and runs k-fold
    cross-validation: for each fold i in 1...k, trains a classifier on
    all the data except the ith fold, and tests on the ith fold.
    Calculates and returns the k-fold cross-validation performance metric for
    classifier clf by averaging the performance across folds.
    Input:
        clf: an instance of SVC()
        X: (n,d) array of feature vectors, where n is the number of examples
           and d is the number of features
        y: (n,) array of binary labels {1,-1}
        k: an int specifying the number of folds (default=5)
        metric: string specifying the performance metric (default='accuracy'
             other options: 'f1-score', 'auroc', 'precision', 'sensitivity',
             and 'specificity')
    Returns:
        average 'test' performance across the k folds as np.float64
    """
    # TODO: Implement this function
    # HINT: You may find the StratifiedKFold from sklearn.model_selection
    # to be useful

    # Put the performance of the model on each fold in the scores array
    scores = []

    skf = StratifiedKFold(n_splits=k)
    skf.get_n_splits(X, y)
    
    for train_index, test_index in skf.split(X, y):
        X_train, X_test = X[train_index], X[test_index]
        y_train, y_test = y[train_index], y[test_index]
        clf.fit(X_train, y_train)
        if metric == "auroc":
            scores.append(performance(y_test, clf.decision_function(X_test), metric = metric))
        else:
            scores.append(performance(y_test, clf.predict(X_test), metric = metric))

    return np.array(scores).mean()


def select_param_linear(
    X, y, k=5, metric="accuracy", C_range=[], loss="hinge", penalty="l2", dual=True
):
    """Search for hyperparameters of linear SVM with best k-fold CV performance.

    Sweeps different settings for the hyperparameter of a linear-kernel SVM,
    calculating the k-fold CV performance for each setting on X, y.
    Input:
        X: (n,d) array of feature vectors, where n is the number of examples
        and d is the number of features
        y: (n,) array of binary labels {1,-1}
        k: int specifying the number of folds (default=5)
        metric: string specifying the performance metric (default='accuracy',
             other options: 'f1-score', 'auroc', 'precision', 'sensitivity',
             and 'specificity')
        C_range: an array with C values to be searched over
        loss: string specifying the loss function used (default="hinge",
             other option of "squared_hinge")
        penalty: string specifying the penalty type used (default="l2",
             other option of "l1")
        dual: boolean specifying whether to use the dual formulation of the
             linear SVM (set True for penalty "l2" and False for penalty "l1"ß)
    Returns:
        the parameter value for a linear-kernel SVM that maximizes the
        average 5-fold CV performance.
    """
    # TODO: Implement this function
    # HINT: You should be using your cv_performance function here
    # to evaluate the performance of each SVM
    bestc = 0
    c_val = 0
    print(metric)
    for c in C_range:
        clf = LinearSVC(C = c, random_state = 445, loss = loss, penalty = penalty, dual = dual)
        perf = cv_performance(clf, X, y, k, metric)
        print(c, " and ", perf)
        if perf > c_val:
            c_val = perf
            bestc = c
    return bestc



def plot_weight(X, y, penalty, C_range, loss, dual):
    """Create a plot of the L0 norm learned by a classifier for each C in C_range.

    Input:
        X: (n,d) array of feature vectors, where n is the number of examples
        and d is the number of features
        y: (n,) array of binary labels {1,-1}
        penalty: penalty to be forwarded to the LinearSVC constructor
        C_range: list of C values to train a classifier on
        loss: loss function to be forwarded to the LinearSVC constructor
        dual: whether to solve the dual or primal optimization problem, to be
            forwarded to the LinearSVC constructor
    Returns: None
        Saves a plot of the L0 norms to the filesystem.
    """
    norm0 = []
    # TODO: Implement this part of the function
    # Here, for each value of c in C_range, you should
    # append to norm0 the L0-norm of the theta vector that is learned
    # when fitting an L2- or L1-penalty, degree=1 SVM to the data (X, y)

    for c in C_range:
        clf = LinearSVC(C = c, random_state = 445, loss = loss, penalty = penalty, dual = dual)
        clf.fit(X,y)
        num = 0
        for ele in clf.coef_[0]:
            if ele != 0:
                num = num +1
        norm0.append(num)



    plt.plot(C_range, norm0)
    plt.xscale("log")
    plt.legend(["L0-norm"])
    plt.xlabel("Value of C")
    plt.ylabel("Norm of theta")
    plt.title("Norm-" + penalty + "_penalty.png")
    plt.savefig("Norm-" + penalty + "_penalty.png")
    plt.close()


def select_param_quadratic(X, y, k=5, metric="accuracy", param_range=[]):
    """Search for hyperparameters of quadratic SVM with best k-fold CV performance.

    Sweeps different settings for the hyperparameters of an quadratic-kernel SVM,
    calculating the k-fold CV performance for each setting on X, y.
    Input:
        X: (n,d) array of feature vectors, where n is the number of examples
           and d is the number of features
        y: (n,) array of binary labels {1,-1}
        k: an int specifying the number of folds (default=5)
        metric: string specifying the performance metric (default='accuracy'
                 other options: 'f1-score', 'auroc', 'precision', 'sensitivity',
                 and 'specificity')
        param_range: a (num_param, 2)-sized array containing the
            parameter values to search over. The first column should
            represent the values for C, and the second column should
            represent the values for r. Each row of this array thus
            represents a pair of parameters to be tried together.
    Returns:
        The parameter values for a quadratic-kernel SVM that maximize
        the average 5-fold CV performance as a pair (C,r)
    """
    # TODO: Implement this function
    # Hint: This will be very similar to select_param_linear, except
    # the type of SVM model you are using will be different...
    best_C_val, best_r_val = 0.0, 0.0
    c_val = 0

    print(metric)
    for c,r in param_range:
        clf = SVC(kernel="poly", degree=2, C=c, coef0=r, gamma='auto', random_state = 445)
        perf = cv_performance(clf, X, y, k=k, metric = metric)
        print(c, " and ", r, " and ", perf)
        if perf > c_val:
            c_val = perf
            best_C_val = c
            best_r_val = r
    return best_C_val, best_r_val




def main():
    # Read binary data
    # NOTE: READING IN THE DATA WILL NOT WORK UNTIL YOU HAVE FINISHED
    #       IMPLEMENTING generate_feature_matrix AND extract_dictionary
    X_train, Y_train, X_test, Y_test, dictionary_binary = get_split_binary_data(
        fname="data/dataset.csv"
    )
    IMB_features, IMB_labels, IMB_test_features, IMB_test_labels = get_imbalanced_data(
        dictionary_binary, fname="data/dataset.csv"
    )
    #question 2
    #a
    # There is a slight ability to identify individuals since people say their names and some are very specific about either where they are or what their life is like. "[NAME]" and a couple had place names like "Toronto"
    # This could be used to approperate new posts to see if they would be seen as positive or negative
    # This shouldn't be used to aproximate posts for a different website or a very different topic since they are likely to have a confilicting topic or different underlying messages.
    print(sk.__version__)
    print(np.__version__)
    print(pd.__version__)
    print(mpl.__version__)
    # TODO: Questions 3, 4, 5
    #3a
    print(extract_word('''BEST book ever! It's great'''))
    #3b
    print(len(dictionary_binary))
    #3c - sum up the matrix and divide by number of rows(size of column)
    print(np.sum(X_train)/len(X_train))
    spot = np.where(np.sum(X_train, axis = 0) == np.amax(np.sum(X_train, axis = 0)))[0][0]
    print(spot)
    # its the letter i
    print(list(dictionary_binary)[spot])

    print(X_train[spot].sum())

    # question 4
    #4a
    # By having similar traing data between the two classes we maintain that the data isn't directed only by one set. In that an example where there are 100 positive points and only one negative point then only 1 fold will even get a negative point so all the other tests wont even consider the nagatives. But by having equal positives and negatives it porvides a balance that each fold will approperately have similar amounts of positives and negatives.
    #4b default='accuracy'other options: 'f1-score', 'auroc', 'precision', 'sensitivity', and 'specificity'
    C_range = [0.001, 0.01, 0.1, 1, 10, 100, 1000]
    print(select_param_linear(X_train, Y_train, 5, "accuracy", C_range))
    print(select_param_linear(X_train, Y_train, 5, "f1-score", C_range))
    print(select_param_linear(X_train, Y_train, 5, "auroc", C_range))
    print(select_param_linear(X_train, Y_train, 5, "precision", C_range))
    print(select_param_linear(X_train, Y_train, 5, "sensitivity", C_range))
    print(select_param_linear(X_train, Y_train, 5, "specificity", C_range))


    #bar chart

    plot_weight(X_train, Y_train, "l2",C_range,"hinge", True )

    #top5 and bot 5
    
    clf = LinearSVC(C = 0.1, random_state = 445, loss="hinge", penalty="l2", dual=True)
    clf.fit(X_train,Y_train)
    theta = clf.coef_[0]
    top5 = np.argpartition(theta, -5)[-5:]
    for c in top5:
        print(list(dictionary_binary)[c] , theta[c])
    clf2 = LinearSVC(C = 0.1, random_state = 445, loss="hinge", penalty="l2", dual=True)
    clf2.fit(X_train,Y_train)
    theta2 = clf2.coef_[0]
    bot5 = np.argpartition(theta2, 5)[:5]
    for c in bot5:
        print(list(dictionary_binary)[c] , theta[c])

    #different penalty and loss
    C_range2 = [0.001, 0.01, 0.1, 1]
    print(select_param_linear(X_train, Y_train, k=5, metric="auroc", C_range = C_range2, loss="squared_hinge", penalty = "l1", dual=False))
    for c in C_range2:
        clf5 = LinearSVC(C = c, random_state = 445, loss="squared_hinge", penalty = "l1", dual=False)
        clf5.fit(X_train, Y_train)
        print(c, performance(Y_test, clf5.decision_function(X_test), metric= "auroc"))
    plot_weight(X_train, Y_train, "l1",C_range2,"squared_hinge", False)


    #4 quadratic
    """
    input1 = []
    for c in C_range:
        for r in C_range:
            input1.append([c,r])
    best = select_param_quadratic(X_train, Y_train, 5, "auroc", input1)
    clf6 = SVC(kernel="poly", degree=2, C=best[0], coef0=best[1], gamma='auto', random_state = 445)
    clf6.fit(X_train, Y_train)
    print(best, performance(Y_test, clf6.decision_function(X_test), metric = "auroc"))
    
    low = -3
    high = 3
    input2 =[]
    for ele in range(25):
        cr = np.exp(np.random.uniform(low, high, size=None))
        rr = np.exp(np.random.uniform(low, high, size=None))
        input2.append([cr,rr])
    best = select_param_quadratic(X_train, Y_train, 5, "auroc", input2)
    clf7 = SVC(kernel="poly", degree=2, C=best[0], coef0=best[1], gamma='auto', random_state = 445)
    clf7.fit(X_train, Y_train)
    print(best, performance(Y_test, clf7.decision_function(X_test), metric = "auroc"))"""

    #imballanced
    metriceses = ["accuracy", "f1-score", "auroc", "precision", "sensitivity", "specificity"]
    for m in metriceses:
        clf3 = LinearSVC(C = 0.01, random_state = 445, loss = "hinge", penalty = "l2", dual = True, class_weight = {-1:1, 1:10})
        clf3.fit(X_train, Y_train)
        if m == "auroc":
            print(m, performance(Y_test, clf3.decision_function(X_test), metric = m))
        else:
            print(m, performance(Y_test, clf3.predict(X_test), metric = m))

    #full imbalance
    for m in metriceses:
        clf4 = LinearSVC(C = 0.01, random_state = 445, loss = "hinge", penalty = "l2", dual = True, class_weight = {-1:1, 1:1})
        clf4.fit(IMB_features, IMB_labels)
        if m == "auroc":
            print(m, performance(IMB_test_labels, clf3.decision_function(IMB_test_features), metric = m))
        else:
            print(m, performance(IMB_test_labels, clf3.predict(IMB_test_features), metric = m))
    bac = 0
    bw1 = 0
    bw2 = 0
    for w1 in range(6,10):
        for w2 in range(1,5):
            clf8 = LinearSVC(C = 0.01, random_state = 445, loss = "hinge", penalty = "l2", dual = True, class_weight = {-1:w1, 1:w2})
            clf8.fit(IMB_features, IMB_labels)
            perf = performance(IMB_test_labels, clf8.predict(IMB_test_features), metric = "f1-score")
            if perf > bac:
                bac = perf
                bw1 = w1
                bw2 = w2
    print(bw1, bw2)
    for m in metriceses:
        clf8 = LinearSVC(C = 0.01, random_state = 445, loss = "hinge", penalty = "l2", dual = True, class_weight = {-1:bw1, 1:bw2})
        clf8.fit(IMB_features, IMB_labels)
        if m == "auroc":
            print(m, performance(IMB_test_labels, clf8.decision_function(IMB_test_features), metric = m))
        else:
            print(m, performance(IMB_test_labels, clf8.predict(IMB_test_features), metric = m))


    #graph
    clf9 = LinearSVC(C = 0.01, random_state = 445, loss = "hinge", penalty = "l2", dual = True, class_weight = {-1:1, 1:1})
    clf10 = LinearSVC(C = 0.01, random_state = 445, loss = "hinge", penalty = "l2", dual = True, class_weight = {-1:bw1, 1:bw2})
    clf9.fit(IMB_features, IMB_labels)
    clf10.fit(IMB_features, IMB_labels)
    fpr1, tpr1, thresholds1 = metrics.roc_curve(IMB_test_labels, clf9.decision_function(IMB_test_features))
    fpr2, tpr2, thresholds2 = metrics.roc_curve(IMB_test_labels, clf10.decision_function(IMB_test_features))
    roc_auc1 = metrics.auc(fpr1, tpr1)
    roc_auc2 = metrics.auc(fpr2, tpr2)
    plt.title('Receiver Operating Characteristic')
    plt.plot(fpr1, tpr1, 'b', label = 'AUC = %0.2f' % roc_auc1)
    plt.plot(fpr2, tpr2, 'b', label = 'AUC = %0.2f' % roc_auc2)
    plt.legend(loc = 'lower right')
    plt.plot([0, 1], [0, 1],'r--')
    plt.xlim([0, 1])
    plt.ylim([0, 1])
    plt.ylabel('True Positive Rate')
    plt.xlabel('False Positive Rate')
    plt.savefig("Roc_auc.png")
    plt.close()


    # Read multiclass data
    # TODO: Question 6: Apply a classifier to heldout features, and then use
    #       generate_challenge_labels to print the predicted labels
    
    (multiclass_features,
    multiclass_labels,
    multiclass_dictionary) = get_multiclass_training_data()
    
    heldout_features = get_heldout_reviews(multiclass_dictionary)

    """bc1 = 0
    bc2 = 0
    b1 = 0
    b2 = 0
    for c in C_range:
        clf11 = SVC(kernel="linear", C=c, random_state = 445, decision_function_shape = "ovo")
        clf12 = SVC(kernel="linear", C=c, random_state = 445, decision_function_shape = "ovr")
        perf1 = cv_performance(clf11, multiclass_features, multiclass_labels)
        perf2 = cv_performance(clf12, multiclass_features, multiclass_labels)
        if perf1 > b1:
            b1 = perf1
            bc1 = c
        if perf2 > b2:
            b2 = perf2
            bc2 = c
    print(bc1, b1)
    print(bc2, b2)"""
    # for ovo vs ovr they were the same 1 0.7208888888888889 was the output

    #best = select_param_quadratic(multiclass_features, multiclass_labels, 5, "accuracy", input1)
    #print(best)
    # 1000  and  1  and  0.7315555555555556
    """
    bc1 = 0
    b1 = 0
    for c in C_range:
        clf11 = LinearSVC(C = c, random_state = 445, loss="squared_hinge", penalty = "l1", dual=False)
        perf1 = cv_performance(clf11, multiclass_features, multiclass_labels)
        if perf1 > b1:
            b1 = perf1
            bc1 = c
    print(bc1, b1) """
    # 1 0.74 for not change in word list
    # 0.7453333 for up to 3 word phrases included
    # 0.7435555555555555 for increasing number of times included in the single comment
    clf11 = LinearSVC(C = 1, random_state = 445, loss="squared_hinge", penalty = "l1", dual=False)
    perf1 = cv_performance(clf11, multiclass_features, multiclass_labels)
    print(perf1)
    #^ works the best so far
    generate_challenge_labels(clf11.predict(heldout_features), "")
    #almost completely done



if __name__ == "__main__":
    main()
