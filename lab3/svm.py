import numpy as np
import scipy as scp

from scipy import sparse as sp
from scipy.sparse import linalg


class PrimalSVM():

    def __init__(self, l2reg=1.0,):
        self.l2reg = l2reg
        self.newton_iter = 20
        self._prec = 1e-6

        self.coef_ = None
        self.support_vectors = None

    def fit(self, X, Y):

        self._X = X
        self._Y = Y

        self._solve_CG(X, Y)

        return self
        

    def _solve_CG(self, X, Y):
        
        [n, d] = X.shape

        self.w = np.zeros(d + 1)

        self.out = np.ones(n)

        l = self.l2reg
        iter = 0

        sv = np.where(self.out > 0)[0]

        mv2 = lambda v: self._matvec_mull(v, sv)
        hess_vec = linalg.LinearOperator((d + 1, d + 1), matvec=mv2)

        while True:
            iter = iter + 1
            if iter > self.newton_iter:
                print("Maximum {0} of Newton steps reached, change newton_iter parameter or try larger lambda".format(
                    iter))
                break

            obj, grad = self._obj_func(self.w, X, Y, self.out)

            sv = np.where(self.out > 0)[0]

            step, info = linalg.minres(hess_vec, -grad)

            t, self.out = self._line_search(self.w, step, self.out)

            self.w += t * step

            if -step.dot(grad) < self._prec * obj:
                break

    def _matvec_mull(self, v, sv):

        X = self._X
        l = self.l2reg

        y = l * v
        y[-1] = 0

        z = X.dot(v[0:-1]) + v[-1]
        zz = np.zeros(z.shape[0])

        zz[sv] = z[sv]
        y = y + np.append(zz.dot(X), zz.sum())

        return y

    def _obj_func(self, w, X, Y, out):
        
        l2reg = self.l2reg

        bias = w[-1]
        w[-1] = 0

        max_out = np.fmax(0, out)
        obj = np.sum(max_out ** 2) / 2 + l2reg * w.dot(w) / 2

        grad = l2reg * w - np.append([np.dot(max_out * Y, X)], [np.sum(max_out * Y)])

        w[-1] = bias

        return (obj, grad)

    def _line_search(self, w, d, out):
        Xd = self._X.dot(d[0:-1]) + d[-1]
        wd = self.l2reg * w[0:-1].dot(d[0:-1])
        dd = self.l2reg * d[0:-1].dot(d[0:-1])

        Y = self._Y

        t = 0
        iter = 0
        out2 = out

        while iter < 1000:
            out2 = out - t * (Y * Xd)
            sv = np.where(out2 > 0)[0]

            g = wd + t * dd - (out2[sv] * Y[sv]).dot(Xd[sv])
            h = dd + Xd[sv].dot(Xd[sv])
            t = t - g / h

            if g ** 2 / h < 1e-10:
                break
            iter = iter + 1

        return t, out2

    def predict(self, X):

        w = self.w[0:-1]
        b = self.w[-1]

        scores = X.dot(w) + b

        prediction = np.sign(scores)

        return prediction, scores