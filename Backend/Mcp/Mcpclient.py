import json
import asyncio
import sys
from contextlib import AsyncExitStack
from typing import Optional
from jose import jwt
from langchain_mcp_adapters.tools import load_mcp_tools
from langchain_openai import ChatOpenAI
from mcp import ClientSession, StdioServerParameters
from mcp.client.stdio import stdio_client
from langgraph.prebuilt import create_react_agent
from langchain_core.messages.ai import AIMessage

sys.stdout.reconfigure(encoding='utf-8')

def extract_user_id(token: str) -> int:
    with open(r"..\TaskManage\TaskManage\appsettings.json", "r") as f:
        config = json.load(f)
    jwt_config = config["Jwt"]
    key = jwt_config["Key"]
    issuer = jwt_config["Issuer"]
    audience = jwt_config["Audience"]
#######
    try:
        # 解码 JWT
        payload = jwt.decode(token, key, algorithms=["HS256"], issuer=issuer, audience=audience)
        print(f"Decoded payload: {payload}")  # 打印解码后的 payload，检查是否包含 user_id 或 sub
        user_id = int(
            payload.get("uid") or
            payload.get("sub") or
            payload.get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
        )
        print(f"Extracted user_id: {user_id}")
        return user_id
    except Exception as e:
        print(f"Error decoding JWT: {e}")
        return None


class MCPClient:
    def __init__(self, token: str, user_id: int, exit_stack: Optional[AsyncExitStack] = None):
        self.token = token
        self.user_id = user_id
        self.session: Optional[ClientSession] = None
        self.exit_stack = exit_stack if exit_stack else AsyncExitStack()
        self.llm = ChatOpenAI(
            api_key="sk-c78e054c1b1c4c0981f904ffd40ef90c",
            base_url="https://dashscope.aliyuncs.com/compatible-mode/v1",
            model="qwen-max"
        )

    async def connect_to_server(self, server_script_path: str):
        server_params = StdioServerParameters(
            command="python",
            args=[server_script_path],
            env={
                "AUTH_TOKEN": f"Bearer {self.token}",
                "USER_ID": str(self.user_id)
            }
        )
        stdio, write = await self.exit_stack.enter_async_context(stdio_client(server_params))
        self.session = await self.exit_stack.enter_async_context(ClientSession(stdio, write))
        await self.session.initialize()
        print("连接到服务器成功")

    async def process_query(self, query: str) -> str:
        tools = await load_mcp_tools(self.session)
        agent = create_react_agent(self.llm, tools)
        agent_response = await agent.ainvoke({"messages": query})
        print(agent_response)
        response = ""
        for message in agent_response['messages']:
            if isinstance(message, AIMessage):
                response += message.content
        return response

    async def cleanup(self):
        await self.exit_stack.aclose()
async def main(query: str, token: str):
        user_id = extract_user_id(token)
        client = MCPClient(token, user_id)
        await client.connect_to_server(
            ".\\Mcpserver.py"
        )
        res = await client.process_query(query)
        print(res)
        await client.cleanup()

if __name__ == "__main__":
    token = token = input("请输入您的 JWT Token：\n")
    query = ("获取一下现在所有的项目列表")
    asyncio.run(main(query, token))

