import sys
import os
import xml.etree.ElementTree as ET
import keras 

def read_datasets(path, num_class):
    folders = os.listdir(path)
    if len(folders) != num_class:
        raise Exception("Invalid class number in datasets")
    for folder in folders:
    
    return X, Y

def read_one_file(path_to_file):
    return X

def train_network(model, X, Y, b_s):
    model.fit(X, Y,epoch=20, batch_size = b_s)
    return model

def predict(model, X):
    return model.predict(X)

def main():
    try:
        if len(sys.argv[1:]) != 1:
            raise Exception("Invalid command arguments")
        tree = ET.parse(sys.argv[1:][0])
        root = tree.getroot()
        if len(root) != 2:
            raise Exception("Invalid file, check file")
        trainpath = root[0].attrib['name']
        input_shape = int(root[1].attrib["size"])
        batch_size = int(root[2].attrib["size"])
        print("Start reading datasets")
        X, Y = read_datasets(trainpath, 3)
        print("Finish reading datasets\nStart creating and training network")
        model = Sequential()
        model.add(Dense(input_shape, input_dim=input_shape,  activation='tanh'))
        model.add(Dense(144,  activation='tanh'))
        model.add(Dense(72,  activation='tanh'))
        model.add(Dense(66,  activation='tanh'))
        model.add(Dense(33,  activation='tanh'))
        model.add(Dense(3, activation='sigmoid'))
        model.compile(optimizer='adadelta', loss='binary_crossentropy', metrics=['accuracy'])
        model = train(model ,X, Y, bacth_size)
        #training
        flag_out = False
        while !flag-out:
            print("Choose option from list (write number of option)\n1.Recognize image\n2.Re-train network\n3.Exit ")
            choose = read()
            if choose == "1":
                print("Write path to image: ")
                pred = read_one_file(read())
                print(predict(model, pred))
            elif choose == "2":
                X, Y = read_datasets(trainpath, 3)
                model = train(model ,X, Y, bacth_size)
            else:
                flag_out = True

        
    except Exception as ex:
        print(ex)


if __name__ == "__main__":
    main()