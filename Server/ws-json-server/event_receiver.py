class EventReceiver:
    def on_accept(self, websocket):
        raise NotImplementedError("on_accept must be implemented by a subclass")

    def on_close(self, websocket):
        raise NotImplementedError("on_close must be implemented by a subclass")

    def on_receive(self, websocket, data: dict):
        raise NotImplementedError("on_receive must be implemented by a subclass")