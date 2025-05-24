from typing import Any, List, Dict
from mcp.server.fastmcp import FastMCP
import datetime

mcp = FastMCP("project-management")

# 内存模拟数据
projects: Dict[int, dict] = {}
tasks: Dict[int, dict] = {}
meetings: List[dict] = []

@mcp.tool()
async def create_project(name: str, description: str = "") -> str:
    """创建新项目"""
    pid = len(projects) + 1
    projects[pid] = {"id": pid, "name": name, "description": description, "tasks": []}
    return f"项目已创建: {name} (ID: {pid})"

@mcp.tool()
async def create_task(project_id: int, title: str, deadline: str = "", priority: int = 1, owner: str = "") -> str:
    """为指定项目创建任务"""
    if project_id not in projects:
        return "项目不存在"
    tid = len(tasks) + 1
    task = {"id": tid, "title": title, "deadline": deadline, "priority": priority, "owner": owner, "status": "TODO"}
    tasks[tid] = task
    projects[project_id]["tasks"].append(tid)
    return f"任务已创建: {title} (ID: {tid})"

@mcp.tool()
async def update_task_status(task_id: int, status: str) -> str:
    """更新任务状态（TODO/DOING/DONE）"""
    if task_id not in tasks:
        return "任务不存在"
    tasks[task_id]["status"] = status
    return f"任务{task_id}状态已更新为{status}"

@mcp.tool()
async def add_meeting_record(project_id: int, content: str) -> str:
    """添加会议记录"""
    record = {"project_id": project_id, "content": content, "time": datetime.datetime.now().isoformat()}
    meetings.append(record)
    return "会议记录已保存"

@mcp.tool()
async def get_project_overview(project_id: int) -> str:
    """获取项目进度总览"""
    if project_id not in projects:
        return "项目不存在"
    task_ids = projects[project_id]["tasks"]
    total = len(task_ids)
    done = sum(1 for tid in task_ids if tasks[tid]["status"] == "DONE")
    return f"项目{project_id}进度：{done}/{total}已完成"

if __name__ == "__main__":
    mcp.run(transport="stdio")