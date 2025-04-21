"""
Target CNN
    Constructs a pytorch model for a convolutional neural network
    Usage: from model.target import target
"""
import torch
import torch.nn as nn
import torch.nn.functional as F
from math import sqrt
from utils import config


class Target(nn.Module):
    def __init__(self):
        super().__init__()

        ## TODO: define each layer
        self.conv1 = nn.Conv2d(3,16,5, stride = 2, padding = 2)
        self.pool = nn.MaxPool2d(2,2)
        self.conv2 = nn.Conv2d(16,64,5, stride = 2, padding = 2)
        self.conv3 = nn.Conv2d(64,8,5, stride = 2, padding = 2) #64
        self.fc_1 = nn.Linear(32,2) #256 for 64 input
        ##

        self.init_weights()

    def init_weights(self):
        torch.manual_seed(42)

        for conv in [self.conv1, self.conv2, self.conv3]:
            C_in = conv.weight.size(1)
            nn.init.normal_(conv.weight, 0.0, 1 / sqrt(5 * 5 * C_in))
            nn.init.constant_(conv.bias, 0.0)

        ## TODO: initialize the parameters for [self.fc_1]
        nn.init.normal_(self.fc_1.weight, 0.0, 1/32)
        nn.init.constant_(self.fc_1.bias,0.0)
        ##

    def forward(self, x):
        """ You may optionally use the x.shape variables below to resize/view the size of
            the input matrix at different points of the forward pass
        """
        N, C, H, W = x.shape
        ## TODO: forward pass
        z = F.relu(self.conv1(x))
        z = self.pool(z)
        z = F.relu(self.conv2(z))
        z = self.pool(z)
        z = F.relu(self.conv3(z))
        z = z.view(-1, 32) #256 for 64 input
        z = self.fc_1(z)
        ##

        return z
