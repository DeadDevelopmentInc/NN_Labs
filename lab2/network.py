import numpy as np

def log(x):
    return 1 / (1 + np.exp(-1 * x))
def d_log(x):
    return log(x) * ( 1 - log(x))

def tanh(x):
    return np.tanh(x)
def d_tanh(x):
    return 1 - np.tanh(x) ** 2 

def ReLu(x):
    mask = (x > 0.0) * 1.0
    return x * mask
def d_ReLu(x):
    mask = (x > 0.0) * 1.0
    return mask    

def elu(matrix):
    mask = (matrix<=0) * 1.0
    less_zero = matrix * mask
    safe =  (matrix>0) * 1.0
    greater_zero = matrix * safe
    final = 3.0 * (np.exp(less_zero) - 1) * less_zero
    return greater_zero + final
def d_elu(matrix):
    safe = (matrix>0) * 1.0
    mask2 = (matrix<=0) * 1.0
    temp = matrix * mask2
    final = (3.0 * np.exp(temp))*mask2
    return (matrix * safe) + final

def iterate_minibatches(inputs, targets, batchsize, shuffle=False):
    assert inputs.shape[0] == targets.shape[0]
    if shuffle:
        indices = np.arange(inputs.shape[0])
        np.random.shuffle(indices)
    for start_idx in range(0, inputs.shape[0] - batchsize + 1, batchsize):
        if shuffle:
            excerpt = indices[start_idx:start_idx + batchsize]
        else:
            excerpt = slice(start_idx, start_idx + batchsize)
        yield inputs[excerpt], targets[excerpt]

class Network:
    w1 = np.random.randn(1875,144) * 0.1
    w2 =np.random.randn(144,72) * 0.1
    w3 =np.random.randn(72,36) * 0.1
    w4 =np.random.randn(36,18) * 0.1
    w5 =np.random.randn(18,9) * 0.1
    w6 =np.random.randn(9,3)  * 0.1
    num_epoch = 10


    def train(self, X, Y, batch_size):
        print('-------------------------')
        AdaDelta_e,AdaDelta_v = 0.0001, 0.146
        AdaDelta_1,AdaDelta_2,AdaDelta_3,AdaDelta_4,AdaDelta_5,AdaDelta_6 = 0,0,0,0,0,0
        AdaDelta_1_v,AdaDelta_2_v,AdaDelta_3_v,AdaDelta_4_v,AdaDelta_5_v,AdaDelta_6_v = 0,0,0,0,0,0
        for iter in range(self.num_epoch):
            total_cost = 0
            for batch in iterate_minibatches(X, Y, batch_size, shuffle=True):
                x_batch, y_batch = batch
                mid_grad_6 = 0
                mid_grad_5 = 0
                mid_grad_4 = 0
                mid_grad_3 = 0
                mid_grad_2 = 0
                mid_grad_1 = 0
                for i in range(len(x_batch)):
                    current_X = np.expand_dims(x_batch[i],axis=0)
                    current_y = np.expand_dims(y_batch[i],axis=0)
                    l1 = current_X.dot(self.w1)
                    l1A = elu(l1)
                    l2 = l1A.dot(self.w2)
                    l2A = tanh(l2) 
                    l3 = l2A.dot(self.w3)
                    l3A = tanh(l3)         
                    l4 = l3A.dot(self.w4)
                    l4A = log(l4)         
                    l5 = l4A.dot(self.w5)
                    l5A = tanh(l5)           
                    l6 = l5A.dot(self.w6)
                    l6A = log(l6) 

                    cost = np.square( l6A - current_y) * 0.5
                    total_cost = total_cost + cost
                    grad_6_part_1 = l6A - current_y
                    grad_6_part_2 = d_log(l6)
                    grad_6_part_3 = l5A        
                    grad_6 = grad_6_part_3.T.dot(grad_6_part_1 * grad_6_part_2)  
        
                    grad_5_part_1 = (grad_6_part_1 * grad_6_part_2).dot(self.w6.T)
                    grad_5_part_2 = d_tanh(l5)
                    grad_5_part_3 = l4A
                    grad_5 =     grad_5_part_3.T.dot(grad_5_part_1 * grad_5_part_2)
        
                    grad_4_part_1 = (grad_5_part_1 * grad_5_part_2).dot(self.w5.T)
                    grad_4_part_2 = d_log(l4)
                    grad_4_part_3 = l3A
                    grad_4 =     grad_4_part_3.T.dot(grad_4_part_1 * grad_4_part_2)  
        
                    grad_3_part_1 = (grad_4_part_1 * grad_4_part_2).dot(self.w4.T)
                    grad_3_part_2 = d_tanh(l3)
                    grad_3_part_3 = l2A
                    grad_3 =     grad_3_part_3.T.dot(grad_3_part_1 * grad_3_part_2)    

                    grad_2_part_1 = (grad_3_part_1 * grad_3_part_2).dot(self.w3.T)
                    grad_2_part_2 = d_tanh(l2)
                    grad_2_part_3 = l1A
                    grad_2 =    grad_2_part_3.T.dot(grad_2_part_1 * grad_2_part_2)

                    grad_1_part_1 = (grad_2_part_1 * grad_2_part_2).dot(self.w2.T)
                    grad_1_part_2 = d_elu(l1)
                    grad_1_part_3 = current_X
                    grad_1 =   grad_1_part_3.T.dot(grad_1_part_1 *grad_1_part_2)

                    AdaDelta_6 = AdaDelta_v * AdaDelta_6 + (1-AdaDelta_v) * grad_6 ** 2
                    AdaDelta_5 = AdaDelta_v * AdaDelta_5 + (1-AdaDelta_v) * grad_5 ** 2
                    AdaDelta_4 = AdaDelta_v * AdaDelta_4 + (1-AdaDelta_v) * grad_4 ** 2
                    AdaDelta_3 = AdaDelta_v * AdaDelta_3 + (1-AdaDelta_v) * grad_3 ** 2
                    AdaDelta_2 = AdaDelta_v * AdaDelta_2 + (1-AdaDelta_v) * grad_2 ** 2
                    AdaDelta_1 = AdaDelta_v * AdaDelta_1 + (1-AdaDelta_v) * grad_1 ** 2

                    mid_grad_6 += - ( np.sqrt(AdaDelta_6_v + AdaDelta_e) / np.sqrt(AdaDelta_6 + AdaDelta_e) ) * grad_6
                    mid_grad_5 += - ( np.sqrt(AdaDelta_5_v + AdaDelta_e) / np.sqrt(AdaDelta_5 + AdaDelta_e) ) * grad_5
                    mid_grad_4 += - ( np.sqrt(AdaDelta_4_v + AdaDelta_e) / np.sqrt(AdaDelta_4 + AdaDelta_e) ) * grad_4
                    mid_grad_3 += - ( np.sqrt(AdaDelta_3_v + AdaDelta_e) / np.sqrt(AdaDelta_3 + AdaDelta_e) ) * grad_3
                    mid_grad_2 += - ( np.sqrt(AdaDelta_2_v + AdaDelta_e) / np.sqrt(AdaDelta_2 + AdaDelta_e) ) * grad_2
                    mid_grad_1 += - ( np.sqrt(AdaDelta_1_v + AdaDelta_e) / np.sqrt(AdaDelta_1 + AdaDelta_e) ) * grad_1

                    AdaDelta_6_v = AdaDelta_v * AdaDelta_6_v + (1-AdaDelta_v) * mid_grad_6 ** 2
                    AdaDelta_5_v = AdaDelta_v * AdaDelta_5_v + (1-AdaDelta_v) * mid_grad_5 ** 2
                    AdaDelta_4_v = AdaDelta_v * AdaDelta_4_v + (1-AdaDelta_v) * mid_grad_4 ** 2
                    AdaDelta_3_v = AdaDelta_v * AdaDelta_3_v + (1-AdaDelta_v) * mid_grad_3 ** 2
                    AdaDelta_2_v = AdaDelta_v * AdaDelta_2_v + (1-AdaDelta_v) * mid_grad_2 ** 2
                    AdaDelta_1_v = AdaDelta_v * AdaDelta_1_v + (1-AdaDelta_v) * mid_grad_1 ** 2

                self.w6 += mid_grad_6
                self.w5 += mid_grad_5
                self.w4 += mid_grad_4
                self.w3 += mid_grad_3
                self.w2 += mid_grad_2
                self.w1 += mid_grad_1
            print("e. Adadelta current Iter: ", iter, " Total Cost: ", total_cost)
            
    
    def predict(self, X):
        l1 = X.dot(self.w1)
        l1A = elu(l1)

        l2 = l1A.dot(self.w2)
        l2A = tanh(l2)       

        l3 = l2A.dot(self.w3)
        l3A = tanh(l3)   
       
        l4 = l3A.dot(self.w4)
        l4A = log(l4) 
        
        l5 = l4A.dot(self.w5)
        l5A = tanh(l5)   
        
        l6 = l5A.dot(self.w6)
        l6A = log(l6) 
        return np.argmax(l6A) + 1