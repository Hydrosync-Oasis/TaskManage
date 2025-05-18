from typing import Any, Optional
from datetime import datetime, timedelta
import uuid
from mcp.server.fastmcp import FastMCP

# 初始化MCP服务器
mcp = FastMCP("project_mgr")

# 内存数据库
projects_db = {}
tasks_db = {}


@mcp.tool()
async def create_project(name: str) -> str:
   #创建新项目

    project_id = str(uuid.uuid4())
    projects_db[project_id] = {
        "id": project_id,
        "name": name,
        "created_at": datetime.now().isoformat()
    }
    return f"项目'{name}'创建成功，ID: {project_id}"


@mcp.tool()
async def create_task(project_id: str, title: str) -> str:
   #创建新任务

    if project_id not in projects_db:
        return "错误：项目不存在"

    task_id = str(uuid.uuid4())
    tasks_db[task_id] = {
        "id": task_id,
        "project_id": project_id,
        "title": title,
        "status": "待办",
        "created_at": datetime.now().isoformat()
    }
    return f"任务'{title}'创建成功"


@mcp.tool()
async def get_tasks(project_id: str) -> str:
   #获取项目所有任务

    project_tasks = [t for t in tasks_db.values()
                     if t["project_id"] == project_id]

    if not project_tasks:
        return "该项目暂无任务"

    return "\n".join(
        f"任务ID: {t['id']}\n标题: {t['title']}\n状态: {t['status']}\n"
        for t in project_tasks
    )


# 辅助函数

def _init_test_data():
    """初始化测试数据"""

    # 测试项目
    project_id = str(uuid.uuid4())
    projects_db[project_id] = {
        "id": project_id,
        "name": "演示项目",
        "created_at": datetime.now().isoformat()
    }

    # 测试任务
    tasks_db[str(uuid.uuid4())] = {
        "id": str(uuid.uuid4()),
        "project_id": project_id,
        "title": "初始化项目",
        "status": "进行中",
        "created_at": datetime.now().isoformat()
    }



if __name__ == "__main__":
    _init_test_data()
    mcp.run(transport="stdio")