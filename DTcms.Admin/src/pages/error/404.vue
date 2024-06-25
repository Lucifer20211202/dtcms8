<template>
	<div class="exception">
		<div class="img-box">
			<img src="../../assets/images/404.svg" />
		</div>
		<div class="content">
			<h1>404</h1>
			<div class="desc">抱歉，你访问的页面不存在或仍在开发中</div>
			<div class="action">
				<el-button type="primary" @click="backHome">返回首页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { useRouter } from 'vue-router'
	//使用路由
	const router = useRouter()
	//返回首页
	const backHome = () => {
		localStorage.removeItem('dtcms_admin_menu_url');
		router.push('/');
	}
</script>

<style lang="scss">
	.exception {
		border-radius: 4px;
		display: flex;
		justify-content: center;
		align-items: center;
		height: 100%;
		.img-box {
			display: flex;
			padding-right: 52px;
			width: 200px;
			height: 200px;
			img {
				width: 100%;
			}
		}
		.content {
			h1 {
				color: #434e59;
				font-size: 72px;
				font-weight: 600;
				line-height: 72px;
				margin-bottom: 24px;
			}
			.desc {
				color: #666;
				font-size: 18px;
				line-height: 28px;
				margin-bottom: 16px;
			}
		}
	}
</style>