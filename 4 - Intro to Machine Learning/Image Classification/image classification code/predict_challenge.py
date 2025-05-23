"""

Predict Challenge
    Runs the challenge model inference on the test dataset and saves the
    predictions to disk
"""

import argparse
import torch
import numpy as np
import pandas as pd
import utils
from dataset import get_challenge
from model.challenge import Challenge
from train_common import *
from utils import config

import utils
from sklearn import metrics
from torch.nn.functional import softmax


def predict_challenge(data_loader, model):
    """
    Runs the model inference on the test set and outputs the predictions
    """
    y_score = []
    for X, y in data_loader:
        output = model(X)
        y_score.append(softmax(output.data, dim=1)[:, 1])
    return torch.cat(y_score)


def main(uniqname):
    """Train challenge model."""
    # data loaders
    if check_for_augmented_data("./data"):
        ch_loader, get_semantic_label = get_challenge(
            task="target",
            batch_size=config("challenge.batch_size"), augment = True
        )
    else:
        ch_loader, get_semantic_label = get_challenge(
            task="target",
            batch_size=config("challenge.batch_size"),
        )

    model = Challenge()

    # Attempts to restore the latest checkpoint if exists
    model, _, _ = restore_checkpoint(model, config("challenge.checkpoint"))

    # Evaluate model
    model_pred = predict_challenge(ch_loader, model)

    print("saving challenge predictions...\n")
    pd_writer = pd.DataFrame(model_pred, columns=["predictions"])
    pd_writer.to_csv(uniqname + ".csv", index=False, header=False)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--uniqname", required=True)
    args = parser.parse_args()
    main(args.uniqname)
