#!/usr/bin/env python
#
# 非同期WebSocketsサーバー
#
#

import asyncio
import json
import sys
import traceback
import time
import logging
import websockets

thismodule = sys.modules[__name__]

players = set() # コネクションリスト

FPS = 40 # FPS

FRAME_SEC =  1 / 40 # 1フレーム当たりの秒数

HOST = 'localhost' # Host IPアドレス

PORT = 5678 # Port番号


def setup_logger():
    logger = logging.getLogger(__name__)
    logger.setLevel(logging.DEBUG)
    handler = logging.StreamHandler()
    handler.setFormatter(logging.Formatter(
        '## %(message)s'
    ))
    logger.addHandler(handler)
    return logger

logger = setup_logger()


async def on_connected(websocket, path):
    u"""コネクション受け付け
    """
    players.add(websocket)
    logger.info("Connection established({})..".format(len(players)))

    while (True):
        if not websocket.open:
            break
        try:
            # メッセージ処理実行
            await handle(websocket, path)
        except:
            break
        await asyncio.sleep(FRAME_SEC)

    players.remove(websocket)
    logger.info("Connection closed.")


async def broadcast(data):
    u"""ブロードキャスト
    """
    for conn in players:
        await conn.send(data)


async def handle(websocket, path):
    u"""メッセージを処理する
    """
    data = await websocket.recv()

    if not data:
        return
    try:
        message = json.loads(data)
        method = message.get('method')
        if not method:
            raise RuntimeError('Method not found in message.')
        payload = message.get('payload')
        logger.debug('<< method:{}, payload:{}'.format(method, payload))

        # メソッドのディスパッチ
        method_name = '_' + method
        f = getattr(thismodule, method_name, None)
        if not f:
            raise RuntimeError('Callable {} not found in message.'.format(method_name))
        await f(websocket, **payload)

    except Exception as e:
        logger.error(e)
        logger.error(traceback.format_exc())


async def _ping(websocket, name):
    u"""Pingのテスト
    """
    logger.debug('>> ping by {}'.format(name))
    data = dict(
        method='pong'
    )
    await websocket.send(json.dumps(data))


async def _broadcast(websocket, name):
    u"""Broadcastのテスト
    """
    logger.debug(">> boradcast by {}".format(name))
    data = dict(
        method='broadcast'
    )
    await broadcast(json.dumps(data))


start_server = websockets.serve(
    on_connected, HOST, PORT
)


if __name__ == '__main__':
    asyncio.get_event_loop().run_until_complete(start_server)
    asyncio.get_event_loop().run_forever()

