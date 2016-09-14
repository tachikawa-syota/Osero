#!/usr/bin/env python
#
# 非同期WebSocketsクライアント
#
#

import asyncio
import time
import json
import sys
import websockets

thismodule = sys.modules[__name__]


async def p():
    u"""サーバーに3秒間隔でPingする
    """
    async with websockets.connect('ws://localhost:5678') as websocket:
        while (True):
            await asyncio.sleep(3)
            data = {
                'method': 'ping', # サーバーの呼び出すメソッド名
                'payload': {
                    'name': 'JoeStar' # メソッドに渡すパラメータ
                }
            }
            print('> {}'.format(data))
            await websocket.send(json.dumps(data))
            response = await websocket.recv()
            print("< {}".format(response))


async def b():
    u"""サーバーにBroadcastリクエストを送る
    """
    print("boradcast")
    async with websockets.connect('ws://localhost:5678') as websocket:
        data = {
            'method': 'broadcast',
            'payload': {
                'name': 'JoeStar'
            }
        }
        print('> {}'.format(data))
        await websocket.send(json.dumps(data))
        response = await websocket.recv()
        print("< {}".format(response))


async def request():
    name = input("Type a methond name you want to test: ")
    f = getattr(thismodule, name, None)
    if f:
        await f()
    else:
        raise RuntimeError('method not found')


asyncio.get_event_loop().run_until_complete(request())
