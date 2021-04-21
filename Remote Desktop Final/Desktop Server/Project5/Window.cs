a = '10'
b = '2'
s = 10
ab = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'

def more(a, b):
    if len(a) > len(b):
        return True
    if len(a) < len(b):
        return False
    for i in range(len(a)):
        if a[i] > b[i]:
            return True
        if a[i] < b[i]:
            return False
    return True


def _sus(a, sus):
    res = ''
    while a != 0:
        res += ab[a % sus]
        a //= sus
    return res[::-1]

def dv(a, b, sus):
    op = ''
    ind = 0
    res = ''
    f = True
    while f:
        if ind < len(a):
            op += a[ind]
        else:
            op += '0'
        ind += 1
        if more(op, b):
            _op = int(op, sus)
            _b = int(b, sus)
            x = _op // _b
            res += ab[x % sus]
            op = _sus(_op - _b, sus)
            print(_op, _b)
            if op == '0' * len(op) and ind == len(a):
                break
    return res


print(dv('A', '2', 30))