<template>
	<el-col :span="24" class="mab-20">
		<el-card>
			<template #header>
				<div class="card-header clearfix">
					<el-icon><Monitor /></el-icon>
					<span>系统信息</span>
				</div>
			</template>
			<div class="cinfo-box">
				<el-timeline>
					<el-timeline-item color="#0bbd87" :timestamp="props.user.lastIp">本次登录IP</el-timeline-item>
					<el-timeline-item :timestamp="props.user.lastTime">本次登录时间</el-timeline-item>
					<el-timeline-item :timestamp="props.user.addTime">账户注册时间</el-timeline-item>
				</el-timeline>
				<el-divider></el-divider>
				<div class="dl-box clearfix">
					<dl>
						<dt>主站名称</dt>
						<dd>{{props.config.webName}}</dd>
					</dl>
					<dl>
						<dt>主站域名</dt>
						<dd>{{props.config.webUrl}}</dd>
					</dl>
					<dl>
						<dt>公司名称</dt>
						<dd>{{props.config.webCompany}}</dd>
					</dl>
					<dl>
						<dt>当前版本</dt>
						<dd>{{props.config.webVersion}}</dd>
					</dl>
				</div>
			</div>
		</el-card>
	</el-col>
</template>

<script setup>
	import { ref,reactive,nextTick,onMounted,watch } from "vue"
	
	//接收props传值
	const props = defineProps({
		user: {
			type: Object,
			default: () => {
				return {}
			}
		},
		config: {
			type: Object,
			default: () => {
				return {}
			}
		}
	})
</script>