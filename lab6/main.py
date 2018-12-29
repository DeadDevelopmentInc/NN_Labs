import sys
import os
import xml.etree.ElementTree as ET
from keras.models import Sequential
from keras.layers import  Dense
from sklearn import svm 
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
                ar = -1
            if _class == 1:
                ar = 0
            if _class == 2:
                ar = 1
            Y.append(ar)
        _class += 1
    return np.asarray(X), np.asarray(Y)

def reading_datasets(m_path, num_class):
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
    return np.array([np.asarray(thod((misc.imread(path))[:,:,:3].reshape(1875)))])

def main():
    if len(sys.argv[1:]) != 1:
        raise Exception("Invalid command arguments")
    flag = False
    while not flag:
        print("Menu:\n1.NN with Tensorflow and Keras (lab2)\n2.SVM with scikit (lab3)\n3. Exit\nWrite choose")
        choose = input()
        if choose == "1":
            main_NN(sys.argv[1:][0])
        elif choose == "2":
            main_SVM(sys.argv[1:][0])
        else:
            flag = True


def main_SVM(file):
    try:
        tree = ET.parse(file)
        root = tree.getroot()
        if len(root) != 1:
            raise Exception("Invalid file, check file")
        trainpath = root[0].attrib['name']
        print("Start reading datasets")
        X, Y = read_datasets(trainpath, 3)
        print("Finish reading datasets\nStart creating and training network")
        trainer = svm.SVC(gamma='scale')
        trainer.fit(X, Y)
        flag_out = False
        while not flag_out:
            print("===========================================\nChoose option from list (write number of option)\n1.Recognize image\n2.Re-train network\n3.Exit ")
            choose = input()
            if choose == "1":
                print("Write path to image: ")
                pred = read_one_file(input())
                pred = trainer.predict(pred)
                #print(pred, score)
                if pred == -1 :
                    print("Your class is Eyes")
                elif pred == 0:
                    print("Your class is Mouth")
                else: 
                    print("Your class is Nose")
            elif choose == "2":
                X, Y = read_datasets(trainpath, 3)
                trainer.fit(X, Y)
            else:
                flag_out = True
        
    except Exception as ex:
        print(ex)


def main_NN(file):
    try:
        tree = ET.parse(file)
        root = tree.getroot()
        if len(root) != 1:
            raise Exception("Invalid file, check file")
        trainpath = root[0].attrib['name']
        print("Start reading datasets")
        X, Y = reading_datasets(trainpath, 3)
        print("Finish reading datasets\nStart creating and training network")
        model = Sequential()
        model.add(Dense(X.shape[1], input_dim=X.shape[1],  activation='tanh'))
        model.add(Dense(144,  activation='tanh'))
        model.add(Dense(72,  activation='tanh'))
        model.add(Dense(66,  activation='tanh'))
        model.add(Dense(33,  activation='tanh'))
        model.add(Dense(3, activation='sigmoid'))
        model.compile(optimizer='adadelta', loss='binary_crossentropy', metrics=['accuracy'])
        model.fit(X, Y, batch_size = 10, epochs=10)
        #training
        flag_out = False
        while not flag_out:
            print("Choose option from list (write number of option)\n1.Recognize image\n2.Re-train network\n3.Exit ")
            choose = input()
            if choose == "1":
                print("Write path to image: ")
                pred = read_one_file(input())
                pred = model.predict(pred)
                max_val = np.argmax(pred[0])
                if max_val == 0:
                    print("Your class is Eyes")
                elif max_val == 1:
                    print("Your class is Mouth")
                else: 
                    print("Your class is Nose")
            elif choose == "2":
                X, Y = read_datasets(trainpath, 3)
                model.fit(X, Y, bacth_size = 10)
            else:
                flag_out = True
        
    except Exception as ex:
        print(ex)


if __name__ == "__main__":
    main()