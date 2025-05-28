import requests
import json
from mcp.server.fastmcp import FastMCP
from fastapi import Request, HTTPException, Depends
from jose import JWTError, jwt
from typing import Dict
from datetime import datetime
from pydantic import BaseModel
from typing import List

# 从 appsettings.json 加载 JWT 配置
with open(r"..\TaskManage\TaskManage\Properties\appsettings.json", "r") as f:
    jwt_config = json.load(f)["Jwt"]

SECRET_KEY = jwt_config["Key"]
ALGORITHM = "HS256"
AUDIENCE = jwt_config["Audience"]
ISSUER = jwt_config["Issuer"]

# 用户 ID -> client & token 映射
user_clients: Dict[int, str] = {}
user_tokens: Dict[int, str] = {}

mcp = FastMCP("project-management")

async def get_user_id_from_token(request: Request) -> int:
    token = request.headers.get("Authorization")
    if not token or not token.startswith("Bearer "):
        raise HTTPException(status_code=401, detail="缺少或错误的认证信息")

    token_value = token.split(" ")[1]
    try:
        payload = jwt.decode(token_value, SECRET_KEY, algorithms=[ALGORITHM], audience=AUDIENCE, issuer=ISSUER)
        user_id = payload.get("uid")
        if user_id is None:
            raise HTTPException(status_code=401, detail="Token中不包含用户ID")
        user_clients[user_id] = f"Client for user {user_id}"
        user_tokens[user_id] = token  # 存原始 Bearer token
        return user_id
    except JWTError as e:
        raise HTTPException(status_code=401, detail=f"Token解码失败: {str(e)}")

# ---------------- Schema ------------------

class ProjectCreateRequest(BaseModel):
    id: int
    name: str
    description: str
    ownerUid: int
    createdAt: datetime

class ProjectUpdateRequest(ProjectCreateRequest):
    pass

class TaskCreateRequest(BaseModel):
    id: int
    title: str
    description: str
    deadline: datetime
    status: int
    priority: int
    projectId: int
    assignedUid: int
    createUserId: int
    dependencyTaskIds: List[int]

class TaskUpdateRequest(TaskCreateRequest):
    pass

class CommentAddRequest(BaseModel):
    commentId: int
    taskId: int
    userId: int
    content: str
    createdTime: datetime

# ---------------- 工具函数 ------------------

@mcp.tool()
async def create_project(body: ProjectCreateRequest, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = "http://localhost:7062/api/Project/Create"
    resp = requests.post(url, json=body.model_dump(), headers={"Authorization": user_tokens[user_id]})
    return "项目创建成功" if resp.status_code == 200 else f"创建失败: {resp.text}"

@mcp.tool()
async def get_all_projects(user_id: int = Depends(get_user_id_from_token)) -> str:
    url = "https://localhost:7062/api/Project/AllProjects"
    resp = requests.get(url, headers={"Authorization": user_tokens[user_id]})
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

@mcp.tool()
async def get_project_by_id(id: int, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = f"https://localhost:7062/api/Project/{id}"
    resp = requests.get(url, headers={"Authorization": user_tokens[user_id]})
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

@mcp.tool()
async def update_project(body: ProjectUpdateRequest, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = f"https://localhost:7062/api/Project/Update/{body.id}"
    resp = requests.put(url, json=body.model_dump(), headers={"Authorization": user_tokens[user_id]})
    return "项目信息更新成功" if resp.status_code == 200 else f"更新失败: {resp.text}"

@mcp.tool()
async def delete_project(id: int, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = f"https://localhost:7062/api/Project/Delete/{id}"
    resp = requests.delete(url, headers={"Authorization": user_tokens[user_id]})
    return "删除成功" if resp.status_code == 200 else f"删除失败: {resp.text}"

@mcp.tool()
async def get_project_tasks(id: int, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = f"https://localhost:7062/api/Project/{id}/Tasks"
    resp = requests.get(url, headers={"Authorization": user_tokens[user_id]})
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

@mcp.tool()
async def create_task(body: TaskCreateRequest, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = "https://localhost:7062/api/Task/Add"
    resp = requests.post(url, json=body.model_dump(), headers={"Authorization": user_tokens[user_id]})
    return "任务创建成功" if resp.status_code == 200 else f"创建失败: {resp.text}"

@mcp.tool()
async def update_task(body: TaskUpdateRequest, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = "https://localhost:7062/api/Task/Update"
    resp = requests.put(url, json=body.model_dump(), headers={"Authorization": user_tokens[user_id]})
    return "任务信息更新成功" if resp.status_code == 200 else f"更新失败: {resp.text}"

@mcp.tool()
async def delete_task(id: int, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = f"https://localhost:7062/api/Task/Delete/{id}"
    resp = requests.delete(url, headers={"Authorization": user_tokens[user_id]})
    return "删除成功" if resp.status_code == 200 else f"删除失败: {resp.text}"

@mcp.tool()
async def get_comment(id: int, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = f"https://localhost:7062/api/Comment/{id}"
    resp = requests.get(url, headers={"Authorization": user_tokens[user_id]})
    if resp.status_code == 200:
        return resp.text
    elif resp.status_code == 404:
        return "评论不存在"
    else:
        return f"获取失败: {resp.text}"

@mcp.tool()
async def add_comment(body: CommentAddRequest, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = "https://localhost:7062/api/Comment/Add"
    resp = requests.post(url, json=body.model_dump(), headers={"Authorization": user_tokens[user_id]})
    return "评论添加成功" if resp.status_code == 200 else f"添加失败: {resp.text}"

@mcp.tool()
async def get_task_comments(id: int, user_id: int = Depends(get_user_id_from_token)) -> str:
    url = f"https://localhost:7062/api/Task/{id}/Comments"
    resp = requests.get(url, headers={"Authorization": user_tokens[user_id]})
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

