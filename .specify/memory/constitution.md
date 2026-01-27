# chatter-chirper Constitution

> 版本：v1.0
> 适用范围：产品需求、技术方案、Mod / 插件、工具链
> 目标：用**可执行、可审计、可演进**的方式写规格（Spec），而不是写说明书。

---

## 第一章｜总则（Preamble）

SpecKit 是一套**规格优先（Spec-first）**的开发宪章，用于指导从想法到交付的全过程。

Spec 不是文档，而是：

* 决策边界（Decision Boundary）
* 协作契约（Collaboration Contract）
* 行为约束（Behavior Constraint）

任何实现都必须**服从 Spec，而非反之**。

---

## 第二章｜Spec 的地位（Supremacy of Spec）

**第 2.1 条**  Spec 是唯一权威来源（Single Source of Truth）。

* 代码、实现、README、发布说明均不得与 Spec 冲突。
* 如有冲突，以 Spec 为准。

**第 2.2 条**  没有 Spec 的功能，视为不存在。

**第 2.3 条**  Spec 的修改等同于产品决策，必须显式记录版本与变更原因。

---

## 第三章｜Spec 的组成（Structure）

每一个 Spec **必须**包含以下章节：

1. 背景与目标（Context & Goal）
2. 明确范围（In Scope / Out of Scope）
3. 功能需求（Functional Requirements）
4. 非功能需求（Non‑Functional Requirements）
5. 约束与假设（Constraints & Assumptions）
6. 失败模式（Failure Modes）
7. 里程碑与验收（Milestones & Acceptance）

缺失任一项，即视为不完整 Spec。

---

## 第四章｜需求分级（Requirement Taxonomy）

所有需求必须被明确标注等级：

* **MUST**：不可缺失，否则 Spec 失败
* **SHOULD**：强烈建议，需有理由才能不实现
* **MAY**：可选项，不影响 Spec 成立
* **MUST NOT**：明确禁止

未分级的需求视为无效。

---

## 第五章｜职责分离（Separation of Concerns）

**第 5.1 条**  Spec 不描述“怎么写代码”，只描述“系统必须表现出什么行为”。

**第 5.2 条**  技术方案（Tech Spec）必须独立于实现细节。

**第 5.3 条**  Patch / Hook / Hack 行为必须在 Spec 中显式声明。

---

## 第六章｜可执行性原则（Executability）

一个合格的 Spec 应当：

* 可被实现
* 可被验证
* 可被否定

**无法验证的 Spec 等同于废话。**

每一条 MUST 需求，必须至少有一个验收条件（Acceptance Criteria）。

---

## 第七章｜失败优先（Failure‑First Thinking）

Spec 必须提前定义失败情况，包括但不限于：

* 数据缺失
* 条件不匹配
* 外部依赖失效
* 性能退化

并明确：

* 回退策略（Fallback）
* 默认行为（Default Behavior）

---

## 第八章｜演进与兼容（Evolution & Compatibility）

**第 8.1 条**  Spec 允许演进，但必须向后兼容，除非明确声明 Breaking Change。

**第 8.2 条**  所有 Breaking Change 必须：

* 提前声明
* 标注版本
* 给出迁移路径

---

## 第九章｜命名与语言（Language & Naming）

* Spec 使用精确、去情绪化语言
* 禁止使用“可能”“大概”“差不多”
* 命名必须可追溯（ID / Key / Code）

**模糊语言即是技术债。**

---

## 第十章｜配置优先（Configuration over Code）

只要满足以下条件之一，就必须配置化：

* 可能被用户调整
* 可能随环境变化
* 可能被 A/B 或策略切换

硬编码行为必须在 Spec 中说明理由。

---

## 第十一章｜最小可行 Spec（Minimum Viable Spec）

一个 MVP Spec 至少应：

* 定义 1 个核心目标
* 定义 1 条主路径
* 定义 1 种失败回退

**没有 MVP 的 Spec 不允许进入实现阶段。**

---

## 第十二章｜审计与回溯（Auditability）

Spec 必须支持：

* 回溯“为什么要这么做”
* 定位“这个行为来自哪条需求”

推荐：

* 需求 ID 映射到代码注释
* Commit message 关联 Spec 条款

---

## 第十三章｜发布纪律（Release Discipline）

任何对外发布版本必须：

* 对应一个 Spec 版本
* 标注实现覆盖率（Implemented / Partial / Deferred）

未覆盖需求必须显式标注原因。

---

## 第十四章｜Spec 的否决权（Right to Say No）

Spec 有权否决：

* 为了“方便”而破坏架构的实现
* 为了“酷炫”而引入不确定性的功能

**实现服务于 Spec，而不是开发者的情绪。**

---

## 第十五章｜终章（Closing）

SpecKit 的最终目标不是限制创造力，而是：

> 把“想法”变成
> 可以被别人理解、实现、维护、接手的系统。

**如果你写不出 Spec，说明你还没想清楚。**

—— SpecKit Constitution
