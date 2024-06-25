<template>
	<div class="navbar">
		<div class="head-box">
			<el-skeleton :loading="datas.headLoading" animated>
				<template #template>
					<div class="img-box">
						<el-skeleton-item variant="image" style="width:54px; height:54px" />
					</div>
					<div class="txt-box">
						<el-skeleton-item variant="h3" style="display:block;width:50%" />
						<el-skeleton-item variant="p" style="display:block;margin-top:10px;width:80%" />
					</div>
				</template>
				<template #default>
					<div class="img-box">
						<img v-if="userInfo.avatar" :src="common.loadFile(userInfo.avatar)" />
						<i v-else class="iconfont icon-user-full"></i>
					</div>
					<div class="txt-box">
						<div class="item">
							<span class="text">用户名</span>
							<span class="title">{{userInfo.userName}}</span>
						</div>
						<div class="item">
							<span class="text">用户组</span>
							<span class="title">{{userInfo.groupTitle}}</span>
						</div>
					</div>
				</template>
			</el-skeleton>
		</div>
		<div class="nav-box">
			<el-skeleton :rows="5" :loading="datas.menuLoading" animated>
				<client-only>
					<el-menu
						mode="vertical"
						:unique-opened="true"
						:default-active="datas.defaultActive"
						active-text-color="#ffd04b"
						background-color="#fff"
						text-color="unset"
						router>
						<el-sub-menu index="4">
							<template #title>
								<el-icon><ElIconPostcard /></el-icon>
								<span>资金管理</span>
							</template>
							<el-menu-item index="/account/balance">余额记录</el-menu-item>
							<el-menu-item index="/account/recharge">充值记录</el-menu-item>
							<el-menu-item index="/account/point">积分记录</el-menu-item>
						</el-sub-menu>
						<el-sub-menu index="5">
							<template #title>
								<el-icon><ElIconDocument /></el-icon>
								<span>投搞管理</span>
							</template>
							<el-menu-item index="/account/contribute/edit">立即投稿</el-menu-item>
							<el-menu-item index="/account/contribute">稿件管理</el-menu-item>
						</el-sub-menu>
						<el-sub-menu index="6">
							<template #title>
								<el-icon><ElIconSetting /></el-icon>
								<span>账户安全</span>
							</template>
							<el-menu-item index="/account/setting/info">基本资料</el-menu-item>
							<el-menu-item index="/account/setting/bind">账号绑定</el-menu-item>
							<el-menu-item index="/account/setting/password">修改密码</el-menu-item>
						</el-sub-menu>
						<el-menu-item index="/account/login" @click="exit">
							<el-icon><ElIconSwitchButton /></el-icon>
							<span>退出登录</span>
						</el-menu-item>
					</el-menu>
				</client-only>
			</el-skeleton>
		</div>
	</div>
</template>

<script setup>
	//获取当前用户信息
	const userInfo = await useUser('info')
	
	//定义页面属性
	const datas = reactive({
		headLoading: true,
		menuLoading: true,
		defaultActive: '',
	})
	
	//退出登录
	const exit = () => {
		useToken().remove()
	}
	
	//页面完成后执行
	onMounted(() => {
		datas.headLoading = false
		datas.menuLoading = false
		datas.defaultActive = useRoute().path
	})
</script>