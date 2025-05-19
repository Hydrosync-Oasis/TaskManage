import asyncio
import os
from typing import Optional
from contextlib import AsyncExitStack

from langchain_openai import ChatOpenAI
from mcp import ClientSession, StdioServerParameters
from mcp.client.stdio import stdio_client
from langchain_mcp_adapters.tools import load_mcp_tools


class MCPClient:
    def __init__(self):
        # 初始化MCP客户端
        self.exit_stack = AsyncExitStack()
        self.session: Optional[ClientSession] = None

        # 初始化AI模型
        self.llm = ChatOpenAI(
            api_key=os.getenv("sk-mrrstlrlgqdnwqpxomwfstzuvuulomaeugersyfbfsihnyts"),
            base_url="https://api.openai.com/v1",
            model="gtp-3.5-turbo"
        )

    async def connect_to_server(self, server_script: str, self=None):

        # 配置服务端启动参数
        server_params = StdioServerParameters(
            command="python",
            args=[server_script]
        )

       
        stdio, write = await self.exit_stack.enter_async_context(
            stdio_client(server_params)


        self.session = await self.exit_stack.enter_async_context(
            ClientSession(stdio, write))

        await self.session.initialize()

        # 打印可用工具列表
        tools = (await self.session.list_tools()).tools
        print("可用工具:", [t.name for t in tools])

        async

        def process_query(self, query: str) -> str:

            if not self.session:
                return "错误：未连接到服务端"

            # 加载MCP工具
            tools = await load_mcp_tools(self.session)

            # 让AI模型选择合适工具
            tool_result = self.llm.bind_tools(tools).invoke(query)

            # 如果有工具调用请求时，执行工具的调用
            if tool_result.additional_kwargs.get("tool_calls"):
                tool_call = tool_result.additional_kwargs["tool_calls"][0]["function"]

                response = await self.session.call_tool(
                    tool_call["name"],
                    tool_call["arguments"]
                )
                return response.content[0].text

            return tool_result.content

        async def cleanup(self):
            await self.exit_stack.aclose()

    async def main(self):

        client = MCPClient()
        try:
            # 连接到服务端
            await client.connect_to_server("Backend/Mcp/Mcpsever.py")

            # 处理用户查询
            response = await client.process_query("查看项目123的所有任务")
            print("AI响应:", response)

        finally:
            await client.cleanup()

    if __name__ == "__main__":
        asyncio.run(main())