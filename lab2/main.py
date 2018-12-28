import sys
import os
import xml.etree.ElementTree as ET
from network import Network
from scipy import ndimage
from scipy import misc
import numpy as np

def thod(X, th = 125):
    for i in range(len(X)):
        if X[i] > th:
            X[i] = 1
        else:
            X[i] = 0
    return X

def read_datasets(m_path, num_class):
    folders = os.listdir(m_path)
    X, Y = [], []
    if len(folders) != num_class:
        raise Exception("Invalid class number in datasets")
    _class = 0
    for folder in folders:
        path = "/".join([m_path, folder])
        print(path)
        for file in os.listdir(path):
            X.append(thod((misc.imread("/".join([path, file])))[:,:,:3].reshape(1875)))
            if _class == 0:
                ar = [1,0,0]
            if _class == 1:
                ar = [0,1,0]
            if _class == 2:
                ar = [0,0,1]
            Y.append(ar)
        _class += 1
    return np.asarray(X), np.asarray(Y)

def read_one_file(path):
    return np.asarray(thod((misc.imread(path))[:,:,:3].reshape(1875)))

def train(network, X, Y, batch_size):
    network.train(X, Y, batch_size)
    return network

def predict(network, X):
    pred = network.predict(X)
    return pred


def main():
    try:
        if len(sys.argv[1:]) != 1:
            raise Exception("Invalid command arguments")
        tree = ET.parse(sys.argv[1:][0])
        root = tree.getroot()
        if len(root) != 2:
            raise Exception("Invalid file, check file")
        trainpath = root[0].attrib['name']
        batch_size = int(root[1].attrib['size'])
        print("Start reading datasets")
        X, Y = read_datasets(trainpath, 3)
        print("Finish reading datasets\nStart creating and training network")
        net = Network()
        net = train(net ,X, Y, batch_size)
        flag_out = False
        while not flag_out:
            print("Choose option from list (write number of option)\n1.Recognize image\n2.Re-train network\n3.Exit ")
            choose = input()
            if choose == "1":
                print("Write path to image: ")
                pred = read_one_file(input())
                pred = predict(net, pred)
                if pred == 1 :
                    print("Your class is Eyes")
                elif pred == 2:
                    print("Your class is Mouth")
                else: 
                    print("Your class is Nose")
            elif choose == "2":
                X, Y = read_datasets(trainpath, 3)
                net = train(net ,X, Y, batch_size)
            else:
                flag_out = True
    except Exception as ex:
        print(ex)
        print(ex.args)


if __name__ == "__main__":
    main()