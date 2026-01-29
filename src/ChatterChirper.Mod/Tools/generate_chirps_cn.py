import json
import os
import random


REQUIRED_KEYS = [
    "CHIRP_TREASURE_HUNT",
    "CHIRP_ROCKET_PRODUCTION",
    "CHIRP_LAUNCH_PREPARATION",
    "CHIRP_LAUNCH",
    "CHIRP_RANDOM_BIRTHDAY",
    "CHIRP_TRAFFIC_JAM",
    "CHIRP_POLICY",
    "CHIRP_RANDOM",
    "CHIRP_DISASTER",
    "CHIRP_RANDOM_DISASTERS",
    "CHIRP_SURVIVORFOUND",
    "CHIRP_RANDOM_INMOTION",
    "CHIRP_RANDOM_EXP5",
    "CHIRP_RANDOM_EXP6",
    "CHIRP_RANDOM_EXP7",
    "CHIRP_RANDOM_EXP8",
    "CHIRP_RANDOM_EXP9",
    "CHIRP_RANDOM_EXP10",
    "CHIRP_RANDOM_EXP11",
    "CHIRP_RANDOM_EXP12",
    "CHIRP_RANDOM_EXP13",
    "CHIRP_DEFAULT",
    "CHIRP_ORGANIC_FARMING",
    "CHIRP_PUBLIC_TRANSPORT_EFFICIENCY",
    "CHIRP_DAYCARE_SERVICE",
    "CHIRP_STUDENT_LODGING",
    "CHIRP_ASSISTIVE_TECHNOLOGIES",
    "CHIRP_CHEAP_FLOWERS",
    "CHIRP_MILESTONE_REACHED",
    "CHIRP_NEW_MAP_TILE",
    "CHIRP_NEW_TILE_PLACED",
    "CHIRP_NO_SCHOOLS",
    "CHIRP_NO_HEALTHCARE",
    "CHIRP_LOW_CRIME",
    "CHIRP_HIGH_CRIME",
    "CHIRP_LOW_HEALTH",
    "CHIRP_TRASH_PILING_UP",
    "CHIRP_NO_WATER",
    "CHIRP_SEWAGE",
    "CHIRP_NO_ELECTRICITY",
    "CHIRP_LOW_HAPPINESS",
    "CHIRP_HAPPY_PEOPLE",
    "CHIRP_ATTRACTIVE_CITY",
    "CHIRP_INDUSTRIAL_DEMAND",
    "CHIRP_COMMERCIAL_DEMAND",
    "CHIRP_RESIDENTIAL_DEMAND",
    "CHIRP_FIRE_HAZARD",
    "CHIRP_DEAD_PILING_UP",
    "CHIRP_HIGH_TECH_LEVEL",
    "CHIRP_ABANDONED_BUILDINGS",
    "CHIRP_NEED_MORE_PARKS",
    "CHIRP_POLLUTION",
    "CHIRP_NOISEPOLLUTION",
    "CHIRP_POISONED",
]


TOPIC_HINTS = {
    "CHIRP_TREASURE_HUNT": ["寻宝", "藏宝点", "小道消息"],
    "CHIRP_ROCKET_PRODUCTION": ["火箭生产", "装配线", "航天工厂"],
    "CHIRP_LAUNCH_PREPARATION": ["发射准备", "倒计时", "总检查"],
    "CHIRP_LAUNCH": ["点火发射", "升空", "轨道"],
    "CHIRP_RANDOM_BIRTHDAY": ["生日", "蛋糕", "惊喜派对"],
    "CHIRP_TRAFFIC_JAM": ["堵车", "高峰", "红绿灯"],
    "CHIRP_POLICY": ["新政策", "条例", "市议会"],
    "CHIRP_RANDOM": ["随手一条", "日常", "碎碎念"],
    "CHIRP_DISASTER": ["灾害", "应急", "避难所"],
    "CHIRP_RANDOM_DISASTERS": ["天有不测", "临时预警", "演练"],
    "CHIRP_SURVIVORFOUND": ["搜救", "幸存者", "联络"],
    "CHIRP_RANDOM_INMOTION": ["在路上", "移动中", "小旅程"],
    "CHIRP_RANDOM_EXP5": ["趣事", "冷知识", "彩蛋"],
    "CHIRP_RANDOM_EXP6": ["节庆", "活动", "庆典"],
    "CHIRP_RANDOM_EXP7": ["工作日常", "打卡", "下班"],
    "CHIRP_RANDOM_EXP8": ["校园", "考试", "社团"],
    "CHIRP_RANDOM_EXP9": ["运动", "健身", "步数"],
    "CHIRP_RANDOM_EXP10": ["宠物", "撸猫", "遛狗"],
    "CHIRP_RANDOM_EXP11": ["天气", "阳光", "阵雨"],
    "CHIRP_RANDOM_EXP12": ["美食", "咖啡", "夜宵"],
    "CHIRP_RANDOM_EXP13": ["旅行", "打卡点", "行程"],
    "CHIRP_DEFAULT": ["城市生活", "日常", "随笔"],
    "CHIRP_ORGANIC_FARMING": ["有机农场", "蔬菜", "集市"],
    "CHIRP_PUBLIC_TRANSPORT_EFFICIENCY": ["公共交通", "准点率", "换乘"],
    "CHIRP_DAYCARE_SERVICE": ["托儿", "保育", "幼儿园"],
    "CHIRP_STUDENT_LODGING": ["学生宿舍", "租房", "舍友"],
    "CHIRP_ASSISTIVE_TECHNOLOGIES": ["无障碍", "助残", "坡道"],
    "CHIRP_CHEAP_FLOWERS": ["便宜花", "花市", "小摊"],
    "CHIRP_MILESTONE_REACHED": ["里程碑", "成就解锁", "鼓掌"],
    "CHIRP_NEW_MAP_TILE": ["新地块", "扩张", "开荒"],
    "CHIRP_NEW_TILE_PLACED": ["落地", "开工", "定标"],
    "CHIRP_NO_SCHOOLS": ["缺学位", "教育", "学校不够"],
    "CHIRP_NO_HEALTHCARE": ["看病难", "医疗不足", "排队"],
    "CHIRP_LOW_CRIME": ["治安好", "安全感", "夜跑"],
    "CHIRP_HIGH_CRIME": ["治安差", "报案", "巡逻"],
    "CHIRP_LOW_HEALTH": ["健康低", "感冒", "体检"],
    "CHIRP_TRASH_PILING_UP": ["垃圾堆", "清运", "臭味"],
    "CHIRP_NO_WATER": ["停水", "水压", "水车"],
    "CHIRP_SEWAGE": ["污水", "异味", "管网"],
    "CHIRP_NO_ELECTRICITY": ["停电", "跳闸", "发电站"],
    "CHIRP_LOW_HAPPINESS": ["不开心", "抱怨", "搬家"],
    "CHIRP_HAPPY_PEOPLE": ["好心情", "夸夸", "点赞"],
    "CHIRP_ATTRACTIVE_CITY": ["城市吸引力", "游客", "打卡"],
    "CHIRP_INDUSTRIAL_DEMAND": ["工业用地", "工厂", "岗位"],
    "CHIRP_COMMERCIAL_DEMAND": ["商业需求", "商场", "店铺"],
    "CHIRP_RESIDENTIAL_DEMAND": ["住宅需求", "新盘", "搬家"],
    "CHIRP_FIRE_HAZARD": ["火险", "可燃物", "疏散"],
    "CHIRP_DEAD_PILING_UP": ["殡葬延误", "运尸车", "处理滞后"],
    "CHIRP_HIGH_TECH_LEVEL": ["高科技", "研发", "黑科技"],
    "CHIRP_ABANDONED_BUILDINGS": ["废弃楼", "烂尾", "空置"],
    "CHIRP_NEED_MORE_PARKS": ["公园", "绿地", "放松"],
    "CHIRP_POLLUTION": ["污染", "PM", "臭气"],
    "CHIRP_NOISEPOLLUTION": ["噪音", "分贝", "安静"],
    "CHIRP_POISONED": ["中毒", "疑似", "送医"],
}


def persona_commuter(topic):
    openers = [
        "又遇到", "刚刚被", "今天也被", "一路上全是", "从公司到家都是",
        "出门第一眼就是", "卡在", "被", "还在", "绕路也还是",
    ]
    tails = [
        "，迟到又要扣绩效了。",
        "，真想摸鱼回去睡觉。",
        "，导航都放弃我了。",
        "，外卖都比我先到家。",
        "，我的月票哭了。",
        "，打车也救不了。",
        "，下班心情瞬间没了。",
        "，明天我骑车试试。",
        "，谁来把红绿灯调调？",
        "，求条生命通道吧。",
    ]
    return f"{random.choice(openers)}{random.choice(topic)}{random.choice(tails)}"


def persona_witty(topic):
    setups = [
        "据不可靠消息，", "小道消息说，", "听说隔壁城，", "我一本正经地宣布：",
        "城市传奇又添一条：", "今日份槽点：", "来点冷幽默：", "认认真真开个玩笑：",
        "有点离谱但好笑：", "官方不背锅我背：",
    ]
    mids = [
        "{t}能练就心态超稳", "{t}已经进化为城市打卡点", "{t}配得上纪录片",
        "{t}让我成为哲学家", "{t}需要来点BGM", "{t}值得发周边",
        "{t}让我悟了", "{t}必须颁个奖",
    ]
    tails = [
        "，笑着笑着就哭了。", "，建议排练到春节联欢。", "，要不搞个城市盲盒？",
        "，我投一票当梗王。", "，求别再加戏了。", "，这很城市天际线。",
        "，给市长艾特一下。", "，先笑为敬。",
    ]
    t = random.choice(topic)
    mid = random.choice(mids).format(t=t)
    return f"{random.choice(setups)}{mid}{random.choice(tails)}"


def persona_pragmatic(topic):
    prefixes = [
        "实话实说：", "客观反馈：", "数据视角：", "现场观察：", "建议记录：",
        "今天复盘：", "流程问题：", "优化笔记：", "一点专业意见：", "从系统看：",
    ]
    mids = [
        "当前{t}影响范围扩大", "{t}出现瓶颈", "{t}告警频繁", "{t}处于高风险",
        "{t}服务能力不足", "{t}体验分下降", "{t}资源调配偏紧", "{t}响应时长偏高",
    ]
    actions = [
        "，建议临时增配与分流。", "，需要热备与旁路。", "，优先做容量评估。",
        "，可先做限流缓解。", "，请拉通多部门联动。", "，建议启动应急预案。",
        "，可用数据看板跟进。", "，建议做根因分析。", "，先止血再优化。",
        "，做一个一周回顾。",
    ]
    t = random.choice(topic)
    return f"{random.choice(prefixes)}{random.choice(mids).format(t=t)}{random.choice(actions)}"


def build_generators(key):
    # 针对部分主题微调风格
    topic = TOPIC_HINTS.get(key, ["城市事件"])
    if key in {"CHIRP_HAPPY_PEOPLE", "CHIRP_ATTRACTIVE_CITY", "CHIRP_MILESTONE_REACHED", "CHIRP_HIGH_TECH_LEVEL"}:
        def commuter(t):
            openers = ["今天真顺", "一路顺风", "体验拉满", "城市好看", "幸福感爆棚"]
            tails = ["，给市政点个赞。", "，愿好事常在。", "，相机都不够拍。", "，心情特别好。", "，继续保持！"]
            return f"{random.choice(openers)}，{random.choice(t)}{random.choice(tails)}"

        def witty(t):
            starts = ["夸夸时间：", "彩虹屁来啦：", "自来水营销：", "官方并没有收钱："]
            mids = ["{t}像滤镜一样", "{t}自带柔光", "{t}不需要P图", "{t}配得上明信片"]
            ends = ["，谁来把美图收了。", "，害我相册爆了。", "，游客都不想走了。", "，建议颁个奖！"]
            tt = random.choice(t)
            return f"{random.choice(starts)}{random.choice(mids).format(t=tt)}{random.choice(ends)}"

        def pragmatic(t):
            mids = ["{t}指标提升明显", "{t}NPS走高", "{t}热度攀升", "{t}口碑扩散"]
            ends = ["，可以加大投入。", "，形成良性循环。", "，抓住窗口期。", "，总结可复制打法。"]
            tt = random.choice(t)
            return f"复盘：{random.choice(mids).format(t=tt)}{random.choice(ends)}"

        return [commuter, witty, pragmatic]

    # 灾害/告警类偏严肃
    if key in {"CHIRP_DISASTER", "CHIRP_RANDOM_DISASTERS", "CHIRP_SEWAGE", "CHIRP_FIRE_HAZARD", "CHIRP_DEAD_PILING_UP", "CHIRP_POISONED"}:
        def commuter(t):
            openers = ["先保命要紧：", "别慌，跟指引走：", "邻里互相照应：", "手机电量要留："]
            mids = ["附近{t}加剧", "这边{t}味道更重", "楼下{t}提示不停", "社区{t}广播不断"]
            ends = ["，快去安全点集合。", "，记得带上证件。", "，老人小孩优先。", "，把宠物也带上。"]
            tt = random.choice(t)
            return f"{random.choice(openers)}{random.choice(mids).format(t=tt)}{random.choice(ends)}"

        def witty(t):
            starts = ["梗先收起：", "不开玩笑：", "此刻认真：", "少点花活："]
            mids = ["{t}不是演练", "{t}真的来了", "{t}影响还在扩大", "{t}相关部门已响应"]
            ends = ["，看官方渠道。", "，按指示撤离。", "，不要造谣。", "，互相转告。"]
            tt = random.choice(t)
            return f"{random.choice(starts)}{random.choice(mids).format(t=tt)}{random.choice(ends)}"

        def pragmatic(t):
            mids = ["{t}风控升级为红色", "{t}应急预案已拉起", "{t}现场指挥部到位", "{t}物资调拨中"]
            ends = ["，请按疏散路线撤离。", "，保持通信畅通。", "，非必要不返回。", "，等待进一步通知。"]
            tt = random.choice(t)
            return f"通告：{random.choice(mids).format(t=tt)}{random.choice(ends)}"

        return [commuter, witty, pragmatic]

    return [persona_commuter, persona_witty, persona_pragmatic]


def generate_for_key(key, n=30):
    topic = TOPIC_HINTS.get(key, ["城市事件"])
    gens = build_generators(key)
    out = []
    seen = set()
    tries = 0
    while len(out) < n and tries < n * 50:
        func = gens[len(out) % len(gens)]
        s = func(topic)
        s = s.replace("  ", " ").strip()
        if s and s not in seen:
            out.append(s)
            seen.add(s)
        tries += 1
    if len(out) < n:
        # 填充占位，确保数量
        base = topic[0] if topic else "城市"
        while len(out) < n:
            out.append(f"关于{base}的小记#{len(out)+1}")
    return out


def main():
    random.seed(20260130)
    data = {}
    for key in REQUIRED_KEYS:
        data[key] = generate_for_key(key, 30)

    # 若已存在文件且包含额外键，保留并覆盖同名键为新中文
    root = os.path.dirname(os.path.dirname(__file__))
    res_path = os.path.join(root, "Resources", "messages.json")

    try:
        if os.path.exists(res_path):
            with open(res_path, "r", encoding="utf-8") as f:
                existing = json.load(f)
            for k, v in existing.items():
                if k not in data:
                    data[k] = v
    except Exception:
        pass

    with open(res_path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

    print(f"Wrote {len(data)} keys to {res_path}")


if __name__ == "__main__":
    main()
