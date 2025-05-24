import json
import asyncio
import os, sys
from contextlib import AsyncExitStack
from typing import Optional
from langchain_mcp_adapters.tools import load_mcp_tools
from langchain_openai import ChatOpenAI
from mcp import ClientSession, StdioServerParameters
from mcp.client.stdio import stdio_client
from mcp.client.sse import sse_client
from langgraph.prebuilt import create_react_agent
from langchain_core.messages.ai import AIMessage

sys.stdout.reconfigure(encoding='utf-8')

class MCPClient:
    def __init__(self, exit_stack: Optional[AsyncExitStack] = None):
        self.session: Optional[ClientSession] = None
        self.exit_stack = exit_stack if exit_stack else AsyncExitStack()
        self.llm = ChatOpenAI(
            api_key="*************",
            base_url="https://dashscope.aliyuncs.com/compatible-mode/v1",
            model="qwen-max"
        )


    async def connect_to_server(self, server_script_path: str):
        server_params = StdioServerParameters(
            command="python", args=[server_script_path], env=None
        )
        stdio, write = await self.exit_stack.enter_async_context(
            stdio_client(server_params)
        )
        self.session = await self.exit_stack.enter_async_context(
            ClientSession(stdio, write)
        )
        await self.session.initialize()
        response = await self.session.list_tools()
        tools = response.tools
        print("\nConnected to server with tools:", [tool.name for tool in tools])

    async def connect_to_sse_server(self, url: str):
        stdio, write = await self.exit_stack.enter_async_context(
            sse_client(url=url)
        )
        self.session = await self.exit_stack.enter_async_context(
            ClientSession(stdio, write)
        )
        await self.session.initialize()
        response = await self.session.list_tools()
        tools = response.tools
        print("\nConnected to server with tools:", [tool.name for tool in tools])

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

    async def process_query_without_langgraph(self, query: str) -> str:
        tools = await load_mcp_tools(self.session)
        tool_result = self.llm.bind_tools(tools).invoke(query)
        print(tool_result)
        from mcp.types import TextContent
        result = ""
        if tool_result.additional_kwargs:
            tools = tool_result.additional_kwargs["tool_calls"][0]["function"]
            response = await self.session.call_tool(tools["name"], json.loads(tools["arguments"]))
            print(response.content)
            for item in response.content:
                if isinstance(item, TextContent):
                    result += item.text
        return result

    async def cleanup(self):
        await self.exit_stack.aclose()

async def main(query: str):
    client = MCPClient()
    await client.connect_to_server(
        "C:\Program Files\pycharm\PycharmProjects\TaskManage\Backend\Mcp\Mcpsever.py"
    )
    # await client.connect_to_sse_server("http://localhost:8000")
    res = await client.process_query(query)
    print(res)
    await client.cleanup()

if __name__ == "__main__":
    asyncio.run(main("什么是马克思主义"))